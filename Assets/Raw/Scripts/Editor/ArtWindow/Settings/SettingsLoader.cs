using System;
using UnityEngine;
using EditorWindowTools;
using UnityEditor;

namespace RuntimeArtWay
{
    public interface ISettingsLoader
    {
        ArtWindowSettings Value { get; }
        void Load();

        void DrawButton(Rect rect);
    }

    public class SettingsLoader : ISettingsLoader
    {
        private const string SETTINGS_PATH = "Assets/Raw/Editor/Settings.asset";

        public ArtWindowSettings Value { get; private set; }

        public void Load()
        {
            var result = AssetDatabase.LoadAssetAtPath(SETTINGS_PATH, typeof(ArtWindowSettings)) as ArtWindowSettings;

            if (result == null)
            {
                result = ScriptableObject.CreateInstance<ArtWindowSettings>();
                AssetDatabase.CreateAsset(result, SETTINGS_PATH);
                result = AssetDatabase.LoadAssetAtPath(SETTINGS_PATH, typeof(ArtWindowSettings)) as ArtWindowSettings;
            }

            Value = result;
        }

        public void DrawButton(Rect rect)
        {
            if (Value == null) throw new InvalidOperationException("Can't draw: Settings object is null.");

            if (GUI.Button(rect, "S"))
            {
                var context = new SettingsPopup(Value);
                PopupWindow.Show(rect, context);
            }
        }

        public class SettingsPopup : PopupWindowContent
        {
            private SettingsEditorTool tool;

            public SettingsPopup(ArtWindowSettings settings)
            {
                tool = new SettingsEditorTool(() => settings);
                tool.Show();
            }

            public override void OnClose()
            {
                tool.Hide();
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(200, 100);
            }

            public override void OnGUI(Rect rect)
            {
                GUILayout.BeginArea(rect);
                tool.Draw();
                GUILayout.EndArea();
            }
        }

        public class SettingsEditorTool : AbstractEditorTool<ArtWindowSettings>
        {
            public SettingsEditorTool(Func<ArtWindowSettings> getNewTarget) : base(getNewTarget)
            {
            }

            protected override void OnDraw()
            {
                target.PreviewMaterial =
                    EditorGUILayout.ObjectField(target.PreviewMaterial, typeof(Material), false) as Material;
                target.StorePath = EditorGUILayout.TextField(target.StorePath);
            }
        }
    }
}