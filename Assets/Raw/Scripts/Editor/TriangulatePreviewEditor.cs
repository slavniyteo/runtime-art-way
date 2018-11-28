using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TriangleNet.Data;
using TriangleNet.Tools;
using TriangleNet.Geometry;
using EditorWindowTools;

namespace RuntimeArtWay
{
    [CustomEditor(typeof(Sample))]
    public class TriangulatePreviewEditor : Editor
    {
        private new Sample target
        {
            get { return base.target as Sample; }
        }

        private SettingsLoader settings;

        private ToolBox<Sample> tools;

        public void OnEnable()
        {
            settings = new SettingsLoader();
            settings.Load();

            var layers = new Layers();
            tools = new ToolBox<Sample>()
            {
                layers,
                new Preview(layers, () => settings.Value.PreviewMaterial)
            };
            tools.Show(target);
        }

        public void OnDestroy()
        {
            tools.Hide();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Separator();
            tools.Draw();
        }
    }
}