using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EditorWindowTools;
using RectEx;

namespace RuntimeArtWay {
    public interface IHistory {
        event Action<Sample> onSelect;
        void Add(Sample current);
    }
    public class History : AbstractEditorTool<Sample>, IHistory {

        private LinkedList<Sample> history = new LinkedList<Sample>();
        private int currentIndex = -1;

        public event Action<Sample> onSelect = x => {};

        private Preview preview = new Preview(new Layers(Layer.HandMade), 2);

        private GUIStyle backNormal;
        private GUIStyle backActive;
        private GUIStyle nameTemporary;
        private GUIStyle namePersistent;
        private GUIStyle nameConflict;

        protected override void PrepareGUI(){
            backNormal = new GUIStyle(GUI.skin.box);

            backActive = new GUIStyle(GUI.skin.box);
            backActive.normal.background = TextureGenerator.GenerateBox(10, 10, Color.red);

            nameTemporary = new GUIStyle(GUI.skin.box);
            nameTemporary.normal.background = TextureGenerator.GenerateBox(10, 10, Color.gray);

            namePersistent = new GUIStyle(GUI.skin.box);
            namePersistent.normal.background = TextureGenerator.GenerateBox(10, 10, Color.green);

            nameConflict = new GUIStyle(GUI.skin.box);
            nameConflict.normal.background = TextureGenerator.GenerateBox(10, 10, Color.cyan);
        }

        public void Add(Sample current) {
            if (!history.Contains(current)){
                history.AddFirst(current);
                currentIndex = 0;
            }
            else {
                currentIndex = IndexOf(current);
            }
        }

        private int IndexOf(Sample sample){
            int i = 0;
            foreach (var target in history){
                if (target == sample){
                    return i;
                }
                i++;
            }
            return -1;
        }

        protected override void OnDraw(){
            int i = 0;
            foreach (var target in history) {
                if (target == null) {
                    history.Remove(target);
                    return;
                }

                var style = currentIndex == i ? backActive : backNormal;
                GUILayout.Box("", style, GUILayout.MinHeight(50), GUILayout.ExpandWidth(true));

                var rect = GUILayoutUtility.GetLastRect().Intend(1);
                var rects = rect.CutFromLeft(50);

                preview.StatelessDraw(rects[0].Intend(1), target);

                DrawInfo(rects[1].Intend(1), i, target);

                if (CheckSelection(rect, i, target)){
                    return;
                }
                i++;
            }
        }

        private void DrawInfo(Rect rect, int index, Sample target){
            rect.height = 18;
            var rects = rect.CutFromRight(18);

            target.name = GUI.TextField(rects[0], target.name);

            DrawPersistence(rects[1], target);
        }

        private void DrawPersistence(Rect rect, Sample target){
            bool isPersistent = EditorUtility.IsPersistent(target);
            var path = isPersistent 
                            ? AssetDatabase.GetAssetPath(target) 
                            : string.Format("Assets/{0}.asset", target.name);
            var objectAtPath = AssetDatabase.LoadAssetAtPath(path, typeof(Sample));

            if (objectAtPath != null) {
                if (objectAtPath == target){
                    GUI.Box(rect, "S", namePersistent);
                }
                else {
                    GUI.Box(rect, "S", nameConflict);
                }
            }
            else {
                if (isPersistent){
                    GUI.Box(rect, "S", namePersistent);
                }
                else if (GUI.Button(rect, "S", nameTemporary)){
                    AssetDatabase.CreateAsset(target, path);
                    AssetDatabase.ImportAsset(path);
                    history.Remove(target);
                    target = AssetDatabase.LoadAssetAtPath(path, typeof(Sample)) as Sample;
                    Add(target);
                }
            }
        }

        private bool CheckSelection(Rect rect, int index, Sample target){
            if (Event.current.type == EventType.MouseDown){
                if (currentIndex != index && rect.Contains(Event.current.mousePosition)){
                    onSelect(target);
                    return true;
                }
            }
            return false;
        }

    }
}