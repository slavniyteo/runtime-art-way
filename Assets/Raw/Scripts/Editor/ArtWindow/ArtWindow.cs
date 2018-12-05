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

        private Sample Target => target.Value;

        private IHistory history;
        private ISettingsLoader settings;

        private IEditorTool leftPanel;
        private IEditorTool rightPanel;
        private IEditorTool centerPanel;

        public void OnEnable()
        {
            settings = new SettingsLoader();
            settings.Load();
            var settingsPanel = new SettingsEditorTool(() => settings.Value as ArtWindowSettings);

            target = new Target();
            target.onChange += ShowAllTools;
            target.onReset += HideAllTools;
            history = new History(() => target.Value, settings.Value);
            target.onChange += () => history.Add(target.Value);
            history.onSelect += x => target.Value = x;

            var layers = new Layers(() => target.Value);
            leftPanel = new ToolBox()
            {
                layers,
            };
            rightPanel = new ToolBox()
            {
                settingsPanel,
                history as IEditorTool,
            };

            centerPanel = new ToolBox()
            {
                new Preview(() => target.Value, layers, () => settings.Value.PreviewMaterial),
            };

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
            centerPanel.Hide();
        }

        private void ShowAllTools()
        {
            leftPanel.Show();
            rightPanel.Show();
            centerPanel.Show();
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
            centerPanel.Draw();
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