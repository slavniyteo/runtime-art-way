using System;
using UnityEngine;
using UnityEditor;
using EditorWindowTools;

namespace RuntimeArtWay
{
    public class ArtWindow : EditorWindow
    {
        [MenuItem("ArtWay/ArtWindow")]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<ArtWindow>();
            window.Show();
        }

        private Target target;

        private Sample Target
        {
            get { return target.Value; }
        }

        private IHistory history;
        private ISettingsLoader settings;

        private IEditorTool<Sample> leftPanel;
        private IEditorTool<Sample> rightPanel;
        private IEditorTool<Sample> preview;

        public void OnEnable()
        {
            target = new Target();
            target.onChange += ShowAllTools;
            target.onReset += HideAllTools;
            history = new History(() => settings.Value.StorePath);
            target.onChange += () => history.Add(Target);
            history.onSelect += x => target.Value = x;

            settings = new SettingsLoader();
            settings.Load();

            var layers = new Layers();
            leftPanel = new ToolBox<Sample>()
            {
                layers
            };
            rightPanel = new ToolBox<Sample>()
            {
                history as IEditorTool<Sample>
            };

            preview = new Preview(layers, () => settings.Value.PreviewMaterial);

            history.LoadSavedData();
        }

        public void OnDestroy()
        {
            HideAllTools();
        }

        private void HideAllTools()
        {
            leftPanel.Hide();
            rightPanel.Hide();
            preview.Hide();
        }

        private void ShowAllTools()
        {
            leftPanel.Show(Target);
            rightPanel.Show(Target);
            preview.Show(Target);
        }

        public void OnGUI()
        {
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

        private void DrawLeftPanel()
        {
            GUILayout.BeginVertical(GUILayout.Width(200));
            leftPanel.Draw();
            GUILayout.EndVertical();
        }

        private void DrawCenter()
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            preview.Draw();
            GUILayout.EndVertical();
        }

        private void DrawRightPanel()
        {
            GUILayout.BeginVertical(GUILayout.Width(300), GUILayout.ExpandWidth(true));
            rightPanel.Draw();
            GUILayout.EndVertical();
        }
    }
}