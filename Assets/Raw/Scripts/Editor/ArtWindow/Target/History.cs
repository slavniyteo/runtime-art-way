using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using EditorWindowTools;
using RectEx;
using RuntimeArtWay.Storage;

namespace RuntimeArtWay
{
    public interface IHistory
    {
        event Action<ISample> onSelect;
        void Add(ISample sample);
        void LoadSavedData();
    }

    public class History : AbstractEditorTool<ISample>, IHistory
    {
        private readonly IArtWindowSettings settings;
        private readonly IStorage<List<Sample>> storage;

        private IList<Sample> history => storage.Value;
        private int currentIndex = -1;

        public event Action<ISample> onSelect = x => { };

        private Preview preview;
        private SaveButton saveButton;
        private RecalculateButton recalculateButton;

        private GUIStyle backNormal;
        private GUIStyle backActive;

        public History(Func<ISample> getNewTarget, IArtWindowSettings settings) : base(getNewTarget)
        {
            this.settings = settings;
            storage = new EditorPrefsListStorage<Sample>("ArtWindow_History");
        }

        protected override void OnShow()
        {
            preview = new Preview(getNewTarget, new Layers(getNewTarget, Layer.HandMade), null, null, 2);

            saveButton = new SaveButton(() => settings.StorePath);
            recalculateButton = new RecalculateButton(settings);

            if (history == null) LoadSavedData();
        }

        protected override void OnHide()
        {
            storage.Save();
        }

        public void LoadSavedData()
        {
            if (history != null) throw new InvalidOperationException();
            if (Active) throw new InvalidOperationException();

            storage.Load();

            if (currentIndex < 0 && history.Any())
            {
                currentIndex = 0;
                onSelect(history[0]);
            }
        }

        protected override void PrepareGUI()
        {
            backNormal = new GUIStyle(GUI.skin.box);

            backActive = new GUIStyle(GUI.skin.box);
            backActive.normal.background = TextureGenerator.GenerateBox(10, 10, Color.red);

            saveButton.PrepareGUI();
        }

        public void Add(ISample current)
        {
            if (!history.Contains(current))
            {
                history.Add(current as Sample);
                currentIndex = 0;
            }
            else
            {
                currentIndex = IndexOf(current);
            }
        }

        private int IndexOf(ISample sample)
        {
            int i = 0;
            foreach (var s in history)
            {
                if (ReferenceEquals(s, sample))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        protected override void OnDraw()
        {
            bool isModified = false;
            void onModify() => isModified = true;

            int i = 0;
            foreach (var t in history)
            {
                if (t == null)
                {
                    history.Remove(t);
                    break;
                }

                DrawElement(i, t, onModify);
                i++;

                if (isModified)
                {
                    break;
                }
            }
        }

        private void DrawElement(int index, Sample sample, Action onModify)
        {
            var style = currentIndex == index ? backActive : backNormal;
            GUILayout.Box("", style, GUILayout.MinHeight(50), GUILayout.ExpandWidth(true));

            var rect = GUILayoutUtility.GetLastRect().Intend(1);
            var rects = rect.CutFromLeft(50);

            preview.StatelessDraw(rects[0].Intend(1), sample);

            DrawInfo(rects[1].Intend(1), index, sample, onModify);

            CheckSelection(rect, index, sample, onModify);
        }

        private void DrawInfo(Rect firstLine, int index, Sample sample, Action onModify)
        {
            firstLine.height = 18;
            var rects = firstLine.Row(
                new float[] {1, 0, 0, 0},
                new float[] {0, 18, 18, 18}
            );

            sample.name = GUI.TextField(rects[0], sample.name);

            saveButton.Draw(rects[1], sample, (oldTarget, newTarget) =>
            {
                history.Remove(oldTarget);
                onModify();

                Add(newTarget);
            });

            recalculateButton.Draw(rects[2], sample);

            if (GUI.Button(rects[3], "H"))
            {
                history.Remove(sample);
                onModify();

                if (index == currentIndex)
                {
                    onSelect(history.FirstOrDefault());
                }
            }

            var secondLine = firstLine.MoveDown();
            EditorGUI.LabelField(secondLine, $"Average Step: {sample.AverageStep}");
        }

        private void CheckSelection(Rect rect, int index, ISample sample, Action onModify)
        {
            if (Event.current.type != EventType.MouseDown) return;

            if (currentIndex != index && rect.Contains(Event.current.mousePosition))
            {
                onSelect(sample);
                onModify();
            }
        }
    }
}