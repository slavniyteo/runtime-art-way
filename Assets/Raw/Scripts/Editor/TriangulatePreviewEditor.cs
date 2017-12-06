using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TriangleNet.Data;
using TriangleNet.Tools;
using TriangleNet.Geometry;

namespace RuntimeArtWay {
[CustomEditor(typeof(Sample))]
public class TriangulatePreviewEditor : Editor {

	private bool drawFullPreview = false;

	private new Sample target { get { return base.target as Sample; } }

	private void DrawMeshPreview(Rect rect){
		EditorGUI.DrawRect(rect, Color.gray);

		var mesh = GenerateMesh(rect, target.circuit);

		if (drawFullPreview){
			DrawFullPreview(rect, mesh);
		}
		else {
			DrawSimplePreview(rect, mesh);
		}
	}

	private void DrawSimplePreview(Rect rect, TriangleNet.Mesh mesh){
		foreach (var s in mesh.Segments){
			if (s.GetTriangle(0) == null || s.GetTriangle(1) == null){
				DrawLine(rect, s.GetVertex(0), s.GetVertex(1), Color.blue);
			}
		}
	}

	private void DrawFullPreview(Rect rect, TriangleNet.Mesh mesh){
		foreach (var t in mesh.Triangles){
			DrawLine(rect, t.GetVertex(0), t.GetVertex(1), Color.blue);
			DrawLine(rect, t.GetVertex(0), t.GetVertex(2), Color.blue);
			DrawLine(rect, t.GetVertex(1), t.GetVertex(2), Color.blue);
		}

		List<ISegment> bounds = new List<ISegment>();
		foreach (var s in mesh.Segments){
			if (s.GetTriangle(0) == null || s.GetTriangle(1) == null){
				bounds.Add(s);
			}
		}
		for (int i = 0; i < Mathf.Min(1000, bounds.Count); i++){
			var s = bounds[i];
			var color = new Color((float)i / bounds.Count, 0,0,1);
			DrawLine(rect, s.GetVertex(0), s.GetVertex(1), color);
			DrawVerticle(rect, s.GetVertex(0), color);
		}
	}

	public TriangleNet.Mesh GenerateMesh(Rect bounds, Vector2[] circuit) {
		var verticles = NormilizedVerticles(circuit, bounds);

		var geometry = new TriangleNet.Geometry.InputGeometry(verticles.Count);
		verticles.ForEach(v => geometry.AddPoint(v.x, v.y));
		for (int i = 0; i < verticles.Count - 1; i++){
			geometry.AddSegment(i, i + 1, 1);
		}
		geometry.AddSegment(verticles.Count - 1, 0, 1);

		var mesh = new TriangleNet.Mesh(new TriangleNet.Behavior{
			ConformingDelaunay = true,
			NoBisect = 0
		});
		mesh.Triangulate(geometry);

		return mesh;
	}

	private List<Vector2> NormilizedVerticles(Vector2[] verticles, Rect rect){
		List<Vector2> result = new List<Vector2>();

		var min = verticles.Aggregate((v, res) => new Vector2(Mathf.Min(v.x, res.x), Mathf.Min(v.y, res.y)));
		var max = verticles.Aggregate((v, res) => new Vector2(Mathf.Max(v.x, res.x), Mathf.Max(v.y, res.y)));
		foreach (var vertex in verticles){
			var v = (vertex - min);
			v.y = (v.y / max.y) * (rect.height - 10);
			v.x = (v.x / max.x) * (rect.width - 10);

			result.Add(v);
		}

		return result;
	}

	private void DrawVerticle(Rect rect, Vertex verticle, Color color){
		var position = new Rect(ToVector2(rect, verticle) - Vector2.one * 2.5f, Vector2.one * 5);
		EditorGUI.DrawRect(position, color);
	}

	private void DrawLine(Rect rect, Vertex from, Vertex to, Color color){
		Drawing.DrawLine(ToVector2(rect, from), ToVector2(rect, to), color, 2, false);
	}

	private Vector2 ToVector2(Rect rect, TriangleNet.Data.Vertex vertex){
		return rect.position + new Vector2((float)vertex.X, rect.height - (float)vertex.Y) + Vector2.one * 0.25f;
	}

	private Vector2 ToVector2(Rect rect, Vector2 position){
		return rect.position + new Vector2(position.x, rect.height - position.y) + Vector2.one * 0.25f;
	}

	private void DrawCircuit(Rect rect){
		EditorGUI.DrawRect(rect, Color.gray);

		var circuit = NormilizedVerticles(target.circuit, rect);
		for (int i = 1; i < circuit.Count; i++){
			Drawing.DrawLine(ToVector2(rect, circuit[i - 1]), ToVector2(rect,circuit[i]), Color.red, 2, false);
		}
	}

	public override void OnInspectorGUI(){
		DrawDefaultInspector();

		EditorGUILayout.Separator();
		if (GUILayout.Button(drawFullPreview ? "Draw simple preview" : "Draw full preview")){
			drawFullPreview = ! drawFullPreview;
		}

		var rect = GUILayoutUtility.GetAspectRect(1);
		DrawMeshPreview(rect);

		EditorGUILayout.Separator();
		var circuitRect = GUILayoutUtility.GetAspectRect(1);
		DrawCircuit(circuitRect);

	}
}
}