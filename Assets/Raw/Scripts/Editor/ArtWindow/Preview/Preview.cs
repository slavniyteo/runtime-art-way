using System;
using System.Collections.Generic;
using System.Linq;
using EditorWindowTools;
using TriangleNet.Data;
using TriangleNet.Geometry;
using UnityEditor;
using UnityEngine;

namespace RuntimeArtWay
{
    public class Preview : AbstractEditorTool<PreviewSample>
    {
        private readonly Drawer drawer;
        private readonly Func<Material> getMaterial;
        private readonly Func<Texture2D> getTexture;

        private Material Material => getMaterial();
        private Texture2D Texture => getTexture();

        private readonly ILayers layers;

        private readonly float dotSize;

        private bool fixFactor;

        private Vector2 scrollPosition;
        private float size = 500;
        private float zoom = 1;
        private int fromIndex;
        private int toIndex;


        public Preview(
            Func<ISample> getNewTarget, ILayers layers,
            Func<Material> getMaterial, Func<float> getStepDivider, Func<Texture2D> getTexture,
            float dotSize = 5
        )
            : base(() => new PreviewSample(getNewTarget()))
        {
            drawer = new Drawer(getNewTarget, getStepDivider);
            drawer.onStartDrawing += () => fixFactor = true;
            drawer.onFinishDrawing += () => fixFactor = false;

            this.layers = layers;
            layers.onChange += OnLayersChange;

            this.dotSize = dotSize;

            this.getMaterial = getMaterial;
            this.getTexture = getTexture;
        }

        private void OnLayersChange(Layer oldValue, Layer newValue)
        {
        }

        protected override void OnShow()
        {
            drawer.Show();
        }

        protected override void OnHide()
        {
            drawer.Hide();

            toIndex = 0;
            fromIndex = 0;
            fixFactor = false;
        }

        #region Draw Mesh Preview

        protected override void OnDraw()
        {
            fromIndex = EditorGUILayout.IntSlider("From Index", fromIndex, 0, target.Count);
            fromIndex = Math.Min(fromIndex, toIndex);
            target.FromIndex = fromIndex;

            toIndex = EditorGUILayout.IntSlider("To Index", toIndex, 0, target.Count);
            target.ToIndex = toIndex;

            size = EditorGUILayout.Slider("Size", size, 200, 1000);
            zoom = EditorGUILayout.Slider("Zoom", zoom, 0.1f, 10);

            var position = GUILayoutUtility.GetRect(-1, size);

            var width = position.width - 20;
            var viewRect = new Rect(position.position, new Vector2(width * zoom, width * zoom));

            drawer.Draw(position);

            scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect);
            StatelessDraw(viewRect, target);
            GUI.EndScrollView();

            target.ToIndex = 0;
        }

        public void StatelessDraw(Rect rect, ISample sample)
        {
            if (Event.current.type == EventType.Layout) return;

            EditorGUI.DrawRect(rect, Color.gray);

            if (!sample.IsDrawn) return;

            var factor = Factor(rect, sample.Vertices);

            if ((layers.Value & Layer.HandMade) == Layer.HandMade)
            {
                var verticles = NormalizedVertices(sample.Vertices, factor);
                DrawDots(rect, verticles, Color.red);
            }

            if (layers.Value == Layer.HandMade) return;

            if (!sample.IsPropagated) return;

            if ((layers.Value & Layer.Propogated) == Layer.Propogated)
            {
                var equalDistance = NormalizedVertices(sample.EqualDistance, factor);
                DrawDots(rect, equalDistance, Color.green);
            }

            if (!sample.HasCircuit) return;

            var meshCircuit = NormalizedVertices(sample.Circuit, factor);
            var mesh = ThirdPartyMeshGenerator.Generate(meshCircuit);

            if ((layers.Value & Layer.MeshSegments) == Layer.MeshSegments)
            {
                DrawTriangles(rect, mesh);
            }

            if ((layers.Value & Layer.MeshCircuit) == Layer.MeshCircuit)
            {
                var circuit = NormalizedVertices(sample.Circuit, factor);
                DrawLine(rect, circuit, Color.magenta);
            }

            if ((layers.Value & Layer.MeshVerticles) == Layer.MeshVerticles)
            {
                DrawVertices(rect, mesh, Color.blue);
            }

            if ((layers.Value & Layer.Texture) == Layer.Texture)
            {
                var trueMesh = new MeshGenerator(UvAlgorithm.Mask).Generate(meshCircuit);
                DrawMeshPreview(rect, trueMesh);
            }
        }


        private void DrawMeshPreview(Rect rect, Mesh mesh)
        {
            if (Material == null) return;

            Material.SetTexture("_MainTex", Texture);

            //For details see http://t-machine.org/index.php/2016/03/13/trying-to-paint-a-mesh-in-unity3d-so-hard-it-makes-you-hate-unity/

            var position = new Vector2(
                x: rect.position.x + 2.5f,
                y: rect.position.y + rect.height + 2.5f
            );
            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.Euler(180, 0, 0), Vector3.one);

            if (Material.SetPass(0))
            {
                Graphics.DrawMeshNow(mesh, matrix);
            }
        }

        private void DrawDots(Rect rect, IList<Vector2> line, Color color)
        {
            for (int i = 1; i < line.Count; i++)
            {
                DrawPoint(rect, line[i], color);
            }
        }

        private void DrawLine(Rect rect, List<Vector2> line, Color color)
        {
            for (int i = 1; i < line.Count; i++)
            {
                DrawLine(rect, line[i - 1], line[i], color);
            }
        }

        private void DrawTriangles(Rect rect, TriangleNet.Mesh mesh)
        {
            foreach (var t in mesh.Triangles)
            {
                DrawLine(rect, t.GetVertex(0), t.GetVertex(1), Color.blue);
                DrawLine(rect, t.GetVertex(0), t.GetVertex(2), Color.blue);
                DrawLine(rect, t.GetVertex(1), t.GetVertex(2), Color.blue);
            }
        }

        private void DrawVertices(Rect rect, TriangleNet.Mesh mesh, Color color)
        {
            var vertices = mesh.Vertices.ToList();
            for (int i = 0; i < vertices.Count; i++)
            {
                var resultColor = Color.Lerp(color * 0.5f, color, (float) i / vertices.Count);
                DrawPoint(rect, ToVector2(vertices[i]), resultColor);
            }
        }

        #endregion

        #region Tools

        private Func<Vector2, Vector2> Factor(Rect rect, IReadOnlyCollection<Vector2> vertices)
        {
            Vector2 min;
            Vector2 max;

            if (fixFactor)
            {
                min = Vector2.zero;
                max = rect.size;
            }
            else
            {
                min = vertices.Aggregate((v, res) => new Vector2(Mathf.Min(v.x, res.x), Mathf.Min(v.y, res.y)));
                max = vertices.Aggregate((v, res) => new Vector2(Mathf.Max(v.x, res.x), Mathf.Max(v.y, res.y)));
                max = max - min;
            }

            return pos =>
            {
                Vector2 v = pos - min;
                var factor = new Vector2(
                    x: (rect.width - dotSize * 4) / max.x,
                    y: (rect.height - dotSize * 4) / max.y
                );
                var min_factor = Mathf.Min(factor.x, factor.y);
                return new Vector2(
                    x: v.x * min_factor + dotSize * 2,
                    y: v.y * min_factor + dotSize * 2
                );
            };
        }

        private static List<Vector2> NormalizedVertices(IEnumerable<Vector2> vertices, Func<Vector2, Vector2> factor)
        {
            return vertices.Select(factor).ToList();
        }

        private void DrawPoint(Rect rect, Vector2 pos, Color color)
        {
            var scaledPosition = ToVector2(rect, pos);
            var position = new Rect(scaledPosition - Vector2.one * dotSize / 2, Vector2.one * dotSize);
            EditorGUI.DrawRect(position, color);
        }

        private void DrawLine(Rect rect, Vertex from, Vertex to, Color color)
        {
            DrawLine(rect, ToVector2(from), ToVector2(to), color);
        }

        private void DrawLine(Rect rect, Vector2 from, Vector2 to, Color color)
        {
            Drawing.DrawLine(ToVector2(rect, from), ToVector2(rect, to), color, 2, false);
        }

        private static Vector2 ToVector2(Point vertex)
        {
            return new Vector2((float) vertex.X, (float) vertex.Y);
        }

        private Vector2 ToVector2(Rect rect, Vector2 position)
        {
            return rect.position + new Vector2(position.x, rect.height - position.y) + Vector2.one * dotSize / 2;
        }

        #endregion
    }
}