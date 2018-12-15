using System;
using EditorWindowTools;
using NUnit.Framework;
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
        RequestForShape ActiveRequest { get; }
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

        public BagOfRequests Value
        {
            get => storage.Value;
            private set
            {
                if (value == Value) return;

                storage.Value = value;
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

        public RequestForShape ActiveRequest { get; private set; }

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
            if (Value is null) return;

            var rect = GUILayoutUtility.GetRect(-1, 18);
            var rects = rect.Row(new float[] {1, 0}, new float[] {0, 18});

            if (GUI.Button(rects[0], "+"))
            {
                Value.requests.Add(new RequestForShape());
                EditorUtility.SetDirty(Value);
            }

            saveButton.Draw(rects[1], Value, (oldValue, newValue) => Value = newValue);
        }

        private void DrawRequests()
        {
            if (Value is null) return;

            Value.requests.RemoveAll(r => r is null);
            foreach (var request in Value.requests)
            {
                if (ActiveRequest is null) ActiveRequest = request;

                DrawRequest(request);
            }
        }

        private void DrawRequest(RequestForShape request)
        {
            var backgroundStyle = ReferenceEquals(ActiveRequest, request) ? backActive : backNormal;
            var rect = EditorGUILayout.BeginHorizontal(backgroundStyle);

            var dragRect = GUILayoutUtility.GetRect(18, -1);

            var textureRect = GUILayoutUtility.GetAspectRect(1, texturePreview, GUILayout.MinWidth(36));
            texturePreview.normal.background = request.texture;
            request.texture = EditorGUI.ObjectField(
                textureRect, request.texture, typeof(Texture2D), false) as Texture2D;

            EditorGUILayout.BeginVertical();
            request.label = EditorGUILayout.TextField(request.Label);
            request.uvAlgorithm = (UvAlgorithm) EditorGUILayout.EnumPopup(request.UvAlgorithm);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown
                && rect.Contains(Event.current.mousePosition))
            {
                ActiveRequest = request;
            }
        }

        #endregion
    }
}