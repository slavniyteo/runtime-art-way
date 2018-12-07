using System;
using System.Collections.Generic;
using System.Linq;
using EditorWindowTools;
using RectEx;
using TriangleNet.Data;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.Assertions;

namespace RuntimeArtWay
{
    public class Preview : AbstractEditorTool<PreviewSample>
    {
        private readonly Drawer drawer;
        private readonly Func<Material> getMaterial;

        private readonly ILayers layers;

        private readonly float dotSize;

        private bool fixFactor;

        private Vector2 scrollPosition;
        private float size = 500;
        private float zoom = 1;
        private int fromIndex = 0;
        private int toIndex = 0;

        private Material Material => getMaterial();

        public Preview(Func<ISample> getNewTarget, ILayers layers, Func<Material> getMaterial, float dotSize = 5)
            : base(() => new PreviewSample(getNewTarget()))
        {
            drawer = new Drawer(getNewTarget);
            drawer.onStartDrawing += () => fixFactor = true;
            drawer.onFinishDrawing += () => fixFactor = false;

            this.layers = layers;
            layers.onChange += OnLayersChange;

            this.dotSize = dotSize;

            this.getMaterial = getMaterial;
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

        public void StatelessDraw(Rect rect, ISample target)
        {
            if (Event.current.type == EventType.Layout) return;

            EditorGUI.DrawRect(rect, Color.gray);

            if (!target.IsDrawn) return;

            var factor = Factor(rect, target.Vertices);

            if ((layers.Value & Layer.HandMade) == Layer.HandMade)
            {
                var verticles = NormilizedVerticles(target.Vertices, factor);
                DrawDots(rect, verticles, Color.red);
            }

            if (layers.Value == Layer.HandMade) return;

            if (!target.IsPropagated) return;

            if ((layers.Value & Layer.Propogated) == Layer.Propogated)
            {
                var equalDistance = NormilizedVerticles(target.EqualDistance, factor);
                DrawDots(rect, equalDistance, Color.green);
            }

            if (!target.HasCircuit) return;

            var meshCircuit = NormilizedVerticles(target.Circuit, factor);
            var mesh = ThirdPartyMeshGenerator.Generate(meshCircuit);

            if ((layers.Value & Layer.MeshSegments) == Layer.MeshSegments)
            {
                DrawTriangles(rect, mesh);
            }

            if ((layers.Value & Layer.MeshCircuit) == Layer.MeshCircuit)
            {
                var circuit = NormilizedVerticles(target.Circuit, factor);
                DrawLine(rect, circuit, Color.magenta);
            }

            if ((layers.Value & Layer.MeshVerticles) == Layer.MeshVerticles)
            {
                DrawVerticles(rect, mesh, Color.blue);
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

            //For details see http://t-machine.org/index.php/2016/03/13/trying-to-paint-a-mesh-in-unity3d-so-hard-it-makes-you-hate-unity/

            var position = new Vector2(
                x: rect.position.x,
                y: rect.position.y + rect.height
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
                DrawPoint(rect, ToVector2(rect, line[i]), color);
            }
        }

        private void DrawLine(Rect rect, List<Vector2> line, Color color)
        {
            for (int i = 1; i < line.Count; i++)
            {
                DrawLine(rect, ToVector2(rect, line[i - 1]), ToVector2(rect, line[i]), color);
            }
        }

        private void DrawSegments(Rect rect, TriangleNet.Mesh mesh)
        {
            foreach (var s in mesh.Segments)
            {
                if (s.GetTriangle(0) == null || s.GetTriangle(1) == null)
                {
                    DrawLine(rect, s.GetVertex(0), s.GetVertex(1), Color.blue);
                }
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

        private void DrawVerticles(Rect rect, TriangleNet.Mesh mesh, Color color)
        {
            var vertices = mesh.Vertices.ToList();
            for (int i = 0; i < vertices.Count; i++)
            {
                var resultColor = Color.Lerp(color * 0.5f, color, (float) i / vertices.Count);
                DrawVerticle(rect, vertices[i], resultColor);
            }
        }

        #endregion

        #region Tools

        private Func<Vector2, Vector2> Factor(Rect rect, IEnumerable<Vector2> verticles)
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
                min = verticles.Aggregate((v, res) => new Vector2(Mathf.Min(v.x, res.x), Mathf.Min(v.y, res.y)));
                max = verticles.Aggregate((v, res) => new Vector2(Mathf.Max(v.x, res.x), Mathf.Max(v.y, res.y)));
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

        private List<Vector2> NormilizedVerticles(IEnumerable<Vector2> verticles, Func<Vector2, Vector2> factor)
        {
            return verticles.Select(factor).ToList();
        }

        private void DrawVerticle(Rect rect, Vertex verticle, Color color)
        {
            var position = ToVector2(rect, verticle);
            DrawPoint(rect, position, color);
        }

        private void DrawPoint(Rect rect, Vector2 pos, Color color)
        {
            var position = new Rect(pos - Vector2.one * dotSize / 2, Vector2.one * dotSize);
            EditorGUI.DrawRect(position, color);
        }

        private void DrawLine(Rect rect, Vertex from, Vertex to, Color color)
        {
            DrawLine(rect, ToVector2(rect, from), ToVector2(rect, to), color);
        }

        private void DrawLine(Rect rect, Vector2 from, Vector2 to, Color color)
        {
            Drawing.DrawLine(from, to, color, 2, false);
        }

        private Vector2 ToVector2(Rect rect, Vertex vertex)
        {
            return rect.position + new Vector2((float) vertex.X, rect.height - (float) vertex.Y) +
                   Vector2.one * dotSize / 2;
        }

        private Vector2 ToVector2(Rect rect, Vector2 position)
        {
            return rect.position + new Vector2(position.x, rect.height - position.y) + Vector2.one * dotSize / 2;
        }

        #endregion
    }

    public class PreviewSample : ISample
    {
        private readonly ISample origin;

        public PreviewSample(ISample origin)
        {
            this.origin = origin;
        }

        public int FromIndex { get; set; }
        public int ToIndex { get; set; }

        public List<Vector2> EqualDistance
        {
            get => CutList(origin.EqualDistance);
        }

        public List<Vector2> Circuit
        {
            get => CutList(origin.Circuit);
        }

        private List<Vector2> CutList(List<Vector2> list)
        {
            int fromIndex = GetFromIndex(list.Count);
            int toIndex = GetToIndex(list.Count);

            if (fromIndex == 0 && toIndex == 0) return list;
            return list.GetRange(fromIndex, toIndex - fromIndex + 1);
        }

        private int GetFromIndex(int maxValue)
        {
            return Math.Max(0, Math.Min(ToIndex, Math.Min(FromIndex, maxValue - 1)));
        }

        private int GetToIndex(int maxValue)
        {
            return Math.Max(0, Math.Min(ToIndex, maxValue - 1));
        }

        #region Delegate

        public string name
        {
            get => origin.name;
            set => origin.name = value;
        }

        public int Count => origin.Count;

        public float AverageStep => origin.AverageStep;

        public List<Vector2> Vertices
        {
            get => origin.Vertices;
        }

        public bool IsDrawn => origin.IsDrawn;

        public bool IsPropagated => origin.IsPropagated;

        public bool HasCircuit => origin.HasCircuit;

        #endregion
    }
}