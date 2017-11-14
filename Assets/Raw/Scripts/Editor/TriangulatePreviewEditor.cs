using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TriangleNet.Data;
using TriangleNet.Tools;
using TriangleNet.Geometry;

[CustomEditor(typeof(TriangulatePreview))]
public class TriangulatePreviewEditor : Editor {

	private new TriangulatePreview target { get { return base.target as TriangulatePreview; } }

	public override void DrawPreview(Rect rect){
		EditorGUI.DrawRect(rect, Color.gray);

		var mesh = target.GenerateMesh(rect);
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
		for (int i = 0; i < Mathf.Min(10000, bounds.Count); i++){
			var s = bounds[i];
			var color = new Color((float)i / bounds.Count, 0,0,1);
			DrawLine(rect, s.GetVertex(0), s.GetVertex(1), color);
			DrawVerticle(rect, s.GetVertex(0), color);
		}
	}

	private void DrawVerticle(Rect rect, Vertex verticle, Color color){
		var position = new Rect(ToVector2(rect, verticle) - Vector2.one * 2.5f, Vector2.one * 5);
		EditorGUI.DrawRect(position, color);
	}

	private void DrawLine(Rect rect, Vertex from, Vertex to, Color color){
		Drawing.DrawLine(ToVector2(rect, from), ToVector2(rect, to), color, 2, false);
	}

	private Vector2 ToVector2(Rect rect, TriangleNet.Data.Vertex vertex){
		return rect.position + new Vector2((float)vertex.X, (float)vertex.Y) + Vector2.one * 0.25f;
	}


	public override void OnInspectorGUI(){
		DrawDefaultInspector();

		var rect = GUILayoutUtility.GetAspectRect(1);
		DrawPreview(rect);

	}
}