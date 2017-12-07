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

        private Target target;
        private Sample Target { get { return target.Value; } }

        private IEditorTool<Sample> leftPanel;
        private IEditorTool<Sample> rightPanel;
        private IEditorTool<Sample> preview;

        public void OnEnable(){
            target = new Target();
            target.onChange += Init;

            var layers = new Layers();
            leftPanel = new ToolBox<Sample>(){
                layers
            };
            rightPanel = new ToolBox<Sample>();
            preview = new Preview(layers);
        }

        private void Init(){
            if (Target != null){
                leftPanel.Show(Target);
                rightPanel.Show(Target);
                preview.Show(Target);
            } 
            else {
                leftPanel.Hide();
                rightPanel.Hide();
                preview.Hide();
            }
        }

        public void OnGUI(){
            target.Draw();
            if (Target == null) return;

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