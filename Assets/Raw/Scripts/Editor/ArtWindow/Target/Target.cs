using System;
using UnityEditor;
using UnityEngine;

namespace RuntimeArtWay
{
    public class Target
    {
        public event Action onChange = () => { };
        public event Action onReset = () => { };

        private Sample value;

        public Sample Value
        {
            get { return value; }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    if (value == null)
                    {
                        onReset();
                    }
                    else
                    {
                        onChange();
                    }
                }
            }
        }

        public void Draw()
        {
            DrawSelectLine();

            if (Value == null)
            {
                EditorGUILayout.HelpBox("Select a sample to work", MessageType.Info);
            }
        }

        private void DrawSelectLine()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                Value = Sample.CreateInstance<Sample>();
                Value.name = "New Sample";
            }
            else
            {
                Value = EditorGUILayout.ObjectField(Value, typeof(Sample), false) as Sample;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}