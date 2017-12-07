using System;
using UnityEditor;

namespace RuntimeArtWay {
    public class Target {

        public event Action onChange = () => {};

        public Sample Value { get; private set; }

        public void Draw(){
            var oldValue = Value;
            Value = EditorGUILayout.ObjectField(Value, typeof(Sample), false) as Sample;
            if (Value != oldValue) {
                onChange();
            }

            if (Value == null) {
                EditorGUILayout.HelpBox("Select a sample to work", MessageType.Info);
            }
        }

    }
}