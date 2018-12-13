using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using EditorWindowTools;
using RectEx;

namespace RuntimeArtWay
{
    public interface IHistory
    {
        event Action<ISample> onSelect;
        void Add(ISample current);
        void LoadSavedData();
    }

    public class History : AbstractEditorTool<ISample>, IHistory
    {
        private const String EDITOR_PREFS_KEY = "ArtWindow_History";

        private LinkedList<ISample> history;
        private int currentIndex = -1;

        public event Action<ISample> onSelect = x => { };

        private Preview preview;
        private SaveButton saveButton;
        private RecalculateButton recalculateButton;

        private GUIStyle backNormal;
        private GUIStyle backActive;

        private IArtWindowSettings settings;

        public History(Func<ISample> getNewTarget, IArtWindowSettings settings) : base(getNewTarget)
        {
            this.settings = settings;
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
            SaveToPrefs(history);
        }

        public void LoadSavedData()
        {
            if (history != null) throw new InvalidOperationException();
            if (Active) throw new InvalidOperationException();

            history = LoadFromPrefs();

            if (currentIndex < 0 && history.Any())
            {
                currentIndex = 0;
                onSelect(history.First.Value);
            }
        }

        private static LinkedList<ISample> LoadFromPrefs()
        {
            var result = new LinkedList<ISample>();

            if (EditorPrefs.HasKey(EDITOR_PREFS_KEY))
            {
                var saved = EditorPrefs.GetString(EDITOR_PREFS_KEY)
                    .Split('|')
                    .Select(x => AssetDatabase.LoadAssetAtPath(x, typeof(ISample)) as ISample);

                foreach (var sample in saved)
                {
                    result.AddLast(sample);
                }
            }


            return result;
        }

        private static void SaveToPrefs(LinkedList<ISample> history)
        {
            EditorPrefs.DeleteKey(EDITOR_PREFS_KEY);
            var result = history
                .Where(s => s is Sample)
                .Cast<Sample>()
                .Where(EditorUtility.IsPersistent)
                .Select(AssetDatabase.GetAssetPath)
                .ToArray();

            if (result.Any())
            {
                var toSave = result.Aggregate((x, y) => x + "|" + y);
                EditorPrefs.SetString(EDITOR_PREFS_KEY, toSave);
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
                history.AddFirst(current);
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
                if (s == sample)
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

        private void DrawElement(int index, ISample sample, Action onModify)
        {
            var style = currentIndex == index ? backActive : backNormal;
            GUILayout.Box("", style, GUILayout.MinHeight(50), GUILayout.ExpandWidth(true));

            var rect = GUILayoutUtility.GetLastRect().Intend(1);
            var rects = rect.CutFromLeft(50);

            preview.StatelessDraw(rects[0].Intend(1), sample);

            DrawInfo(rects[1].Intend(1), index, sample, onModify);

            CheckSelection(rect, index, sample, onModify);
        }

        private void DrawInfo(Rect firstLine, int index, ISample sample, Action onModify)
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
                    if (history.Count > 0)
                    {
                        onSelect(history.First.Value);
                    }
                    else
                    {
                        onSelect(null);
                    }
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