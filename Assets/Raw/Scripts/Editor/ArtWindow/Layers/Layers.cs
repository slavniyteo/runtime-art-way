using System;
using EditorWindowTools;
using UnityEditor;
using UnityEngine;

namespace RuntimeArtWay {

    public delegate void ChangeLayersHandler(Layer oldValue, Layer newValue);
    public interface ILayers {
        event ChangeLayersHandler onChange;
        Layer Value { get; }
    }
    public class Layers : AbstractEditorTool<Sample>, ILayers {

        public event ChangeLayersHandler onChange = (x, y) => {};

        public Layer Value { get; private set; }

        public Layers() {
            Value = Layer.HandMade | Layer.MeshCircuit;
        }

        protected override void OnDraw(){
            var layers = Enum.GetNames(typeof(Layer));
            foreach (var layerName in layers){
                DrawLayer(layerName);
            }
        }

        private void DrawLayer(string layerName){
            var value = (Layer) Enum.Parse(typeof(Layer), layerName);
            var isActive = (Value & value) == value;

            EditorGUI.BeginChangeCheck();
            isActive = GUILayout.Toggle(isActive, layerName);

            if (EditorGUI.EndChangeCheck()){
                var oldValue = Value;
                Value = isActive 
                    ? Value | value
                    : Value & ~ value;
                onChange(oldValue, Value);
            }
        }
    }
}