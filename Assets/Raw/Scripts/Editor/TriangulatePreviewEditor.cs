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
    [CustomEditor(typeof(ISample))]
    public class TriangulatePreviewEditor : Editor
    {
        private new ISample target
        {
            get { return base.target as ISample; }
        }

        private SettingsLoader settings;

        private ToolBox tools;

        public void OnEnable()
        {
            settings = new SettingsLoader();
            settings.Load();

            var layers = new Layers(() => target);
            tools = new ToolBox()
            {
                layers,
                new Preview(() => target, layers,
                    () => settings.Value.PreviewMaterial,
                    () => settings.Value.CircuitRelativeStep)
            };
            tools.Show();
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