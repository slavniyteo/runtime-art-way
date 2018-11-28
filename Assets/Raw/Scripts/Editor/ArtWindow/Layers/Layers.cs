using System;
using EditorWindowTools;
using UnityEditor;
using UnityEngine;

namespace RuntimeArtWay
{
    public delegate void ChangeLayersHandler(Layer oldValue, Layer newValue);

    public interface ILayers
    {
        event ChangeLayersHandler onChange;
        Layer Value { get; }
    }

    public class Layers : AbstractEditorTool<Sample>, ILayers
    {
        private static readonly string EDITOR_PREFS_KEY = "ArtWindow_Layers_key";
        private static readonly int DEFAULT_LAYERS = (int) (Layer.HandMade | Layer.MeshCircuit);

        public event ChangeLayersHandler onChange = (x, y) => { };

        private bool isValueFixed;
        private Layer fixedValue;

        public Layer Value
        {
            get { return isValueFixed ? fixedValue : (Layer) EditorPrefs.GetInt(EDITOR_PREFS_KEY, DEFAULT_LAYERS); }
            private set
            {
                if (isValueFixed)
                {
                    fixedValue = value;
                }
                else
                {
                    EditorPrefs.SetInt(EDITOR_PREFS_KEY, (int) value);
                }
            }
        }

        public Layers(Layer value)
        {
            isValueFixed = true;
            this.Value = value;
        }

        public Layers()
        {
            isValueFixed = false;
        }

        protected override void OnDraw()
        {
            var layers = Enum.GetNames(typeof(Layer));
            foreach (var layerName in layers)
            {
                DrawLayer(layerName);
            }
        }

        private void DrawLayer(string layerName)
        {
            var value = (Layer) Enum.Parse(typeof(Layer), layerName);
            var isActive = (Value & value) == value;

            EditorGUI.BeginChangeCheck();
            isActive = GUILayout.Toggle(isActive, layerName);

            if (EditorGUI.EndChangeCheck())
            {
                var oldValue = Value;
                Value = isActive
                    ? Value | value
                    : Value & ~ value;
                onChange(oldValue, Value);
            }
        }
    }
}