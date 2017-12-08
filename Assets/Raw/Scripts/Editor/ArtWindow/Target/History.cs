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

        public void Add(Sample current) {
            if (!history.Contains(current)){
                history.AddFirst(current);
                currentIndex = 0;
            }
            else {
                currentIndex = IndexOf(current);
            }
        }

        protected override void OnDraw(){
            int i = 0;
            foreach (var target in history) {
                EditorGUILayout.BeginHorizontal();
                if (currentIndex == i) GUILayout.Box("!");
                GUILayout.Box(target.name, GUILayout.ExpandWidth(true));

                if (CheckSelection(i, target)){
                    return;
                }

                GUILayout.EndHorizontal();
                i++;
            }
        }

        private bool CheckSelection(int index, Sample target){
            if (Event.current.type == EventType.MouseDown){
                var rect = GUILayoutUtility.GetLastRect();
                if (rect.Contains(Event.current.mousePosition)){
                    onSelect(target);
                    return true;
                }
            }
            return false;
        }

        public Sample this[int i] {
            get {
                if (i >= history.Count) throw new IndexOutOfRangeException();

                int num = 0;
                foreach (var target in history){
                    if (num == i){
                        return target;
                    }
                    num++;
                }
                return null;
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

    }
}