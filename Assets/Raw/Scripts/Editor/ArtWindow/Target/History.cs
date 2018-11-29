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
        event Action<Sample> onSelect;
        void Add(Sample current);
        void LoadSavedData();
    }

    public class History : AbstractEditorTool<Sample>, IHistory
    {
        private const String EDITOR_PREFS_KEY = "ArtWindow_History";

        private LinkedList<Sample> history;
        private int currentIndex = -1;

        public event Action<Sample> onSelect = x => { };

        private Preview preview;
        private SaveButton saveButton;
        private RecalculateButton recalculateButton = new RecalculateButton();

        private GUIStyle backNormal;
        private GUIStyle backActive;

        private Func<String> getStorePath;

        public History(Func<String> getStorePath)
        {
            this.getStorePath = getStorePath;
        }

        protected override void OnShow()
        {
            preview = new Preview(new Layers(Layer.HandMade), null, 2);

            saveButton = new SaveButton(getStorePath);

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

        private static LinkedList<Sample> LoadFromPrefs()
        {
            var result = new LinkedList<Sample>();

            if (EditorPrefs.HasKey(EDITOR_PREFS_KEY))
            {
                var saved = EditorPrefs.GetString(EDITOR_PREFS_KEY)
                    .Split('|')
                    .Select(x => AssetDatabase.LoadAssetAtPath(x, typeof(Sample)) as Sample);

                foreach (var sample in saved)
                {
                    result.AddLast(sample);
                }
            }


            return result;
        }

        private static void SaveToPrefs(LinkedList<Sample> history)
        {
            EditorPrefs.DeleteKey(EDITOR_PREFS_KEY);
            var result = history.Where(EditorUtility.IsPersistent)
                .Select(AssetDatabase.GetAssetPath);
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

        public void Add(Sample current)
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

        private int IndexOf(Sample sample)
        {
            int i = 0;
            foreach (var target in history)
            {
                if (target == sample)
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
            Action onModify = () => isModified = true;
            int i = 0;
            foreach (var target in history)
            {
                if (target == null)
                {
                    history.Remove(target);
                    break;
                }

                int count = history.Count;
                DrawElement(i, target, onModify);
                i++;

                if (isModified)
                {
                    break;
                }
            }
        }

        private void DrawElement(int index, Sample target, Action onModify)
        {
            var style = currentIndex == index ? backActive : backNormal;
            GUILayout.Box("", style, GUILayout.MinHeight(50), GUILayout.ExpandWidth(true));

            var rect = GUILayoutUtility.GetLastRect().Intend(1);
            var rects = rect.CutFromLeft(50);

            preview.StatelessDraw(rects[0].Intend(1), target);

            DrawInfo(rects[1].Intend(1), index, target, onModify);

            CheckSelection(rect, index, target, onModify);
        }

        private void DrawInfo(Rect rect, int index, Sample target, Action onModify)
        {
            rect.height = 18;
            var rects = rect.Row(
                new float[] {1, 0, 0, 0},
                new float[] {0, 18, 18, 18}
            );

            target.name = GUI.TextField(rects[0], target.name);

            saveButton.Draw(rects[1], target, (oldTarget, newTarget) =>
            {
                history.Remove(oldTarget);
                onModify();

                Add(newTarget);
            });

            recalculateButton.Draw(rects[2], target);

            if (GUI.Button(rects[3], "H"))
            {
                history.Remove(target);
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
        }

        private void CheckSelection(Rect rect, int index, Sample target, Action onModify)
        {
            if (Event.current.type != EventType.MouseDown) return;

            if (currentIndex != index && rect.Contains(Event.current.mousePosition))
            {
                onSelect(target);
                onModify();
            }
        }
    }
}