using System;
using UnityEditor;
using UnityEngine;

namespace RuntimeArtWay {
    public class Target {

        public event Action onChange = () => {};

        public Sample Value { get; private set; }

        public void Draw(){
            var oldValue = Value;
            DrawSelectLine();
            if (Value != oldValue) {
                onChange();
            }

            if (Value == null) {
                EditorGUILayout.HelpBox("Select a sample to work", MessageType.Info);
            }
        }

        private void DrawSelectLine(){
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+", GUILayout.Width(20))){
                Value = Sample.CreateInstance<Sample>();
                Value.name = "New Sample";
            }
            else {
                Value = EditorGUILayout.ObjectField(Value, typeof(Sample), false) as Sample;
            }

            EditorGUILayout.EndHorizontal();
        }

    }
}