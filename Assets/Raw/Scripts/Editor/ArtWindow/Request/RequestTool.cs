using System;
using EditorWindowTools;
using RectEx;
using RuntimeArtWay.Storage;
using UnityEditor;
using UnityEngine;

namespace RuntimeArtWay
{
    public interface IRequestTool
    {
        event Action onChange;
        event Action onReset;

        BagOfRequests Value { get; }
    }

    public class RequestTool : IEditorTool, IRequestTool
    {
        private GUIStyle backNormal;
        private GUIStyle backActive;
        private GUIStyle texturePreview;

        public event Action onChange = () => { };
        public event Action onReset = () => { };

        private readonly IStorage<BagOfRequests> storage =
            new EditorPrefsStorage<BagOfRequests>("ArtWindow.BagOfRequests");

        private BagOfRequests value
        {
            get => storage.Value;
            set => storage.Value = value;
        }

        public BagOfRequests Value
        {
            get => value;
            private set
            {
                if (value == this.value) return;

                this.value = value;
                if (value == null)
                {
                    onReset();
                }
                else
                {
                    onChange();
                }
            }
        }

        private SaveButton saveButton;

        #region IEditorTool

        public void Show()
        {
            storage.Load();
        }

        public void Hide()
        {
            storage.Save();
        }

        public void Draw()
        {
            PrepareGUI();

            OnDraw();
        }

        private void PrepareGUI()
        {
            if (backNormal != null) return;
            saveButton = new SaveButton(() => "");
            saveButton.PrepareGUI();

            backNormal = new GUIStyle(GUI.skin.box);

            backActive = new GUIStyle(GUI.skin.box)
            {
                normal = {background = TextureGenerator.GenerateBox(10, 10, Color.red)}
            };

            texturePreview = new GUIStyle(GUI.skin.box);
        }

        #endregion

        #region Draw

        private void OnDraw()
        {
            DrawSelectLine();

            if (Value == null)
            {
                EditorGUILayout.HelpBox("Select a bag to work", MessageType.Info);
                return;
            }

            DrawCommandLine();
            DrawRequests();
        }

        private void DrawSelectLine()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                Value = ScriptableObject.CreateInstance<BagOfRequests>();
                Value.name = "New Sample";
            }

            Value = EditorGUILayout.ObjectField(Value, typeof(BagOfRequests), false) as BagOfRequests;

            EditorGUILayout.EndHorizontal();
        }

        private void DrawCommandLine()
        {
            if (value is null) return;

            var rect = GUILayoutUtility.GetRect(-1, 18);
            var rects = rect.Row(new float[] {1, 0}, new float[] {0, 18});

            if (GUI.Button(rects[0], "+"))
            {
                value.requests.Add(new RequestForShape());
                EditorUtility.SetDirty(value);
            }

            saveButton.Draw(rects[1], value, (oldValue, newValue) => value = newValue);
        }

        private void DrawRequests()
        {
            if (value is null) return;

            value.requests.RemoveAll(r => r is null);
            foreach (var request in value.requests)
            {
                DrawRequest(request);
            }
        }

        private void DrawRequest(RequestForShape request)
        {
            EditorGUILayout.BeginHorizontal(backNormal);

            var textureRect = GUILayoutUtility.GetAspectRect(1, texturePreview, GUILayout.MinWidth(36));
            texturePreview.normal.background = request.texture;
            request.texture = EditorGUI.ObjectField(
                textureRect, request.texture, typeof(Texture2D), false) as Texture2D;

            EditorGUILayout.BeginVertical();
            request.label = EditorGUILayout.TextField(request.Label);
            request.uvAlgorithm = (UvAlgorithm) EditorGUILayout.EnumPopup(request.UvAlgorithm);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}