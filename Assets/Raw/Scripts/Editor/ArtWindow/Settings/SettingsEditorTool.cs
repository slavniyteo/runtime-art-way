using System;
using EditorWindowTools;
using UnityEditor;
using UnityEngine;

namespace RuntimeArtWay
{
    public class SettingsEditorTool : AbstractEditorTool<ArtWindowSettings>
    {
        private Editor editor;

        public SettingsEditorTool(Func<ArtWindowSettings> getNewTarget) : base(getNewTarget)
        {
        }

        protected override void OnShow()
        {
            Editor.CreateCachedEditor(target, null, ref editor);
        }

        protected override void OnDraw()
        {
            editor.OnInspectorGUI();
        }
    }
}