using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EditorWindowTools;

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

        protected override void PrepareGUI(){
            backNormal = new GUIStyle(GUI.skin.box);

            backActive = new GUIStyle(GUI.skin.box);
            backActive.normal.background = TextureGenerator.GenerateBox(10, 10, Color.red);

            nameTemporary = new GUIStyle(GUI.skin.box);
            nameTemporary.normal.background = TextureGenerator.GenerateBox(10, 10, Color.gray);

            namePersistent = new GUIStyle(GUI.skin.box);
            namePersistent.normal.background = TextureGenerator.GenerateBox(10, 10, Color.green);
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
                var style = currentIndex == i ? backActive : backNormal;
                var rect = EditorGUILayout.BeginHorizontal(style);

                preview.DrawOnce(target);
                DrawInfo(i, target);

                if (CheckSelection(rect, i, target)){
                    return;
                }

                GUILayout.EndHorizontal();
                i++;
            }
        }

        private void DrawInfo(int index, Sample target){
            EditorGUILayout.BeginVertical();

            var nameStyle = EditorUtility.IsPersistent(target) ? namePersistent : nameTemporary;
            GUILayout.Box(target.name, nameStyle, GUILayout.ExpandWidth(true));

            EditorGUILayout.EndVertical();
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