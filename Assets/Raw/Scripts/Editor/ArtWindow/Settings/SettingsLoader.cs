using System;
using UnityEngine;
using EditorWindowTools;
using UnityEditor;

namespace RuntimeArtWay {

    public interface ISettingsLoader {
        IArtWindowSettings Value { get; }
        void Load();

        void DrawButton(Rect rect);
    }

    public class SettingsLoader : ISettingsLoader {

        private static readonly string SETTINGS_PATH = "Assets/Raw/Editor/Settings.asset";

        private ArtWindowSettings settings;
        public IArtWindowSettings Value { get { return settings; } }

        public void Load(){
            var result = AssetDatabase.LoadAssetAtPath(SETTINGS_PATH, typeof(ArtWindowSettings)) as ArtWindowSettings;

            if (result == null){
                result = ScriptableObject.CreateInstance<ArtWindowSettings>();
                AssetDatabase.CreateAsset(result, SETTINGS_PATH);
                result = AssetDatabase.LoadAssetAtPath(SETTINGS_PATH, typeof(ArtWindowSettings)) as ArtWindowSettings;
            }

            settings = result;
        }

        public void DrawButton(Rect rect){
            if (settings == null) throw new InvalidOperationException("Can't draw: Settings object is null.");

            if (GUI.Button(rect, "S")){
                var context = new SettingsPopup(settings);
                PopupWindow.Show(rect, context);
            }
        }

        public class SettingsPopup : PopupWindowContent {

            private SettingsEditorTool tool;

            public SettingsPopup(ArtWindowSettings settings){
                tool = new SettingsEditorTool();
                tool.Show(settings);
            }

            public override void OnClose(){
                tool.Hide();
            }

            public override Vector2 GetWindowSize(){
                return new Vector2(200, 100);
            }

            public override void OnGUI(Rect rect){
                GUILayout.BeginArea(rect);
                tool.Draw();
                GUILayout.EndArea();
            }

        }

        public class SettingsEditorTool : AbstractEditorTool<ArtWindowSettings> {
            protected override void OnDraw(){
                target.PreviewMaterial = EditorGUILayout.ObjectField(target.PreviewMaterial, typeof(Material), false) as Material;
                target.StorePath = EditorGUILayout.TextField(target.StorePath);
            }
        }

    }
}