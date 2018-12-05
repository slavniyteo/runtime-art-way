using System;
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
    public class Drawer : AbstractEditorTool<Sample>
    {
        public event Action onStartDrawing = () => { };
        public event Action onFinishDrawing = () => { };

        private SampleBuilder builder;

        private bool isDrawing
        {
            get { return builder != null; }
        }

        private Rect rect;

        public Drawer(Func<Sample> getNewTarget)
            : base(getNewTarget)
        {
        }


        public void Draw(Rect rect)
        {
            this.rect = rect;

            Draw();
        }

        protected override void OnDraw()
        {
            var e = Event.current;

            if (!e.isMouse)
            {
                return;
            }

            var position = new MousePosition(rect, e.mousePosition);

            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    MouseDown(position);
                    break;
                }
                case EventType.MouseDrag:
                {
                    MouseDrag(position);
                    break;
                }
                case EventType.MouseUp:
                {
                    MouseUp(position);
                    break;
                }
                default:
                {
                    Debug.Log("Hmmmm " + e.type);
                    break;
                }
            }
        }

        private void MouseDown(MousePosition position)
        {
            if (target.IsDrawn) return;
            if (isDrawing) MouseUp(position);

            if (!position.IsInsideRect) return;

            builder = SampleBuilder.UpdateSample(target, position.Position);

            onStartDrawing();
            Debug.Log("Begin");
        }

        private void MouseDrag(MousePosition position)
        {
            if (!isDrawing) return;

            builder.Add(position.Position);

            Debug.Log("Update");
        }

        private void MouseUp(MousePosition position)
        {
            if (!isDrawing) return;

            builder.Build(5);

            onFinishDrawing();
            Debug.Log("Finished");
        }

        public class MousePosition
        {
            public Vector2 Position { get; private set; }
            public bool IsInsideRect { get; private set; }

            public MousePosition(Rect rect, Vector2 position)
            {
                IsInsideRect = rect.Contains(position);
                Position = getMousePosition(rect, position);
            }

            private Vector2 getMousePosition(Rect rect, Vector2 position)
            {
                var result = new Vector2(
                    x: position.x - rect.x,
                    y: rect.y + rect.height - position.y
                );

                if (result.x < 0) result.x = 0;
                if (result.x > rect.width) result.x = rect.width;
                if (result.y < 0) result.y = 0;
                if (result.y > rect.height) result.y = rect.height;

                return result;
            }

            private static float Between(float min, float value, float max)
            {
                if (value < min) value = min;
                if (value > max) value = max;
                return value;
            }
        }
    }
}