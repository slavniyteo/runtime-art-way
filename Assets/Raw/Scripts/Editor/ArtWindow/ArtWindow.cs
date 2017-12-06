using System;
using UnityEngine;
using UnityEditor;
using EditorWindowTools;

namespace RuntimeArtWay {
    public class ArtWindow : EditorWindow {

        [MenuItem("ArtWay/ArtWindow")]
        public static void ShowWindow(){
            var window = EditorWindow.GetWindow<ArtWindow>();
            window.Show();
        }

        private Sample Target { get; set; }

        private IEditorTool<Sample> leftPanel;
        private IEditorTool<Sample> rightPanel;
        private IEditorTool<Sample> preview;

        public void OnEnable(){
            Target = ScriptableObject.CreateInstance<Sample>();

            leftPanel = new ToolBox<Sample>();
            rightPanel = new ToolBox<Sample>();
            preview = new Preview();

            Init();
        }

        private void Init(){
            leftPanel.Show(Target);
            rightPanel.Show(Target);
            preview.Show(Target);
        }

        public void OnGUI(){
            GUILayout.BeginVertical();
            GUILayout.Label("Header");
            GUILayout.BeginHorizontal();

            DrawLeftPanel();
            DrawCenter();
            DrawRightPanel();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawLeftPanel(){
            GUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Box(Texture2D.whiteTexture, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            leftPanel.Draw();
            GUILayout.EndVertical();
        }

        private void DrawCenter(){
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            preview.Draw();
            GUILayout.EndVertical();
        }

        private void DrawRightPanel(){
            GUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandWidth(true));
            GUILayout.Box(Texture2D.whiteTexture, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            rightPanel.Draw();
            GUILayout.EndVertical();
        }
    }
}