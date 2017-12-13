using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TriangleNet.Data;
using TriangleNet.Tools;
using TriangleNet.Geometry;
using EditorWindowTools;

namespace RuntimeArtWay {
public class Preview : AbstractEditorTool<Sample> {

	private ILayers layers;
	private bool drawFullPreview = false;

	private float dotSize;

	public Preview(ILayers layers, float dotSize = 5) {
		this.layers = layers;
		layers.onChange += OnLayersChange;

		this.dotSize = dotSize;
	}

	private void OnLayersChange(Layer oldValue, Layer newValue){
	}

	#region Draw Mesh Preview

	protected override void OnDraw(){
		var rect = GUILayoutUtility.GetAspectRect(1);
		EditorGUI.DrawRect(rect, Color.gray);

		if (! target.IsDrawn) return;

		if ((layers.Value & Layer.HandMade) == Layer.HandMade){
			DrawDots(rect, target.verticles, Color.red);
		}
		if ((layers.Value & Layer.Propogated) == Layer.Propogated){
			DrawDots(rect, target.equalDistance, Color.green);
		}

		if (! target.HasCircuit) return;

		var meshCircuit = NormilizedVerticles(target.circuit, rect);
		var mesh = MeshGenerator.Generate(meshCircuit);

		if ((layers.Value & Layer.MeshSegments) == Layer.MeshSegments){
			DrawTriangles(rect, mesh);
		}
		if ((layers.Value & Layer.MeshCircuit ) == Layer.MeshCircuit){
			DrawLine(rect, target.circuit, Color.magenta);
		}
		if ((layers.Value & Layer.MeshVerticles) == Layer.MeshVerticles){
			DrawVerticles(rect, mesh);
		}
	}

	private void DrawDots(Rect rect, Vector2[] line, Color color){
		var circuit = NormilizedVerticles(line, rect);
		for (int i = 1; i < circuit.Count; i++){
			DrawPoint(rect, ToVector2(rect, circuit[i]), color);
		}
	}

	private void DrawLine(Rect rect, Vector2[] line, Color color){
		var circuit = NormilizedVerticles(line, rect);
		for (int i = 1; i < circuit.Count; i++){
			DrawLine(rect, ToVector2(rect, circuit[i - 1]), ToVector2(rect, circuit[i]), color);
		}
	}

	private void DrawSegments(Rect rect, TriangleNet.Mesh mesh){
		foreach (var s in mesh.Segments){
			if (s.GetTriangle(0) == null || s.GetTriangle(1) == null){
				DrawLine(rect, s.GetVertex(0), s.GetVertex(1), Color.blue);
			}
		}
	}

	private void DrawTriangles(Rect rect, TriangleNet.Mesh mesh){
		foreach (var t in mesh.Triangles){
			DrawLine(rect, t.GetVertex(0), t.GetVertex(1), Color.blue);
			DrawLine(rect, t.GetVertex(0), t.GetVertex(2), Color.blue);
			DrawLine(rect, t.GetVertex(1), t.GetVertex(2), Color.blue);
		}
	}

	private void DrawVerticles(Rect rect, TriangleNet.Mesh mesh){
		foreach (var v in mesh.Vertices){
			DrawVerticle(rect, v, Color.blue);
		}
	}

	#endregion

	#region Tools

	private List<Vector2> NormilizedVerticles(Vector2[] verticles, Rect rect){
		List<Vector2> result = new List<Vector2>();

		var min = verticles.Aggregate((v, res) => new Vector2(Mathf.Min(v.x, res.x), Mathf.Min(v.y, res.y)));
		var max = verticles.Aggregate((v, res) => new Vector2(Mathf.Max(v.x, res.x), Mathf.Max(v.y, res.y)));
		max = max - min;
		foreach (var vertex in verticles){
			var v = (vertex - min);
			v.y = (v.y / max.y) * (rect.height - dotSize * 4) + dotSize * 2;
			v.x = (v.x / max.x) * (rect.width - dotSize * 4) + dotSize * 2;

			result.Add(v);
		}

		return result;
	}

	private void DrawVerticle(Rect rect, Vertex verticle, Color color){
		var position = ToVector2(rect, verticle);
		DrawPoint(rect, position, color);
	}

	private void DrawPoint(Rect rect, Vector2 pos, Color color){
		var position = new Rect(pos - Vector2.one * dotSize / 2, Vector2.one * dotSize);
		EditorGUI.DrawRect(position, color);
	}

	private void DrawLine(Rect rect, Vertex from, Vertex to, Color color){
		DrawLine(rect, ToVector2(rect, from), ToVector2(rect, to), color);
	}

	private void DrawLine(Rect rect, Vector2 from, Vector2 to, Color color){
		Drawing.DrawLine(from, to, color, 2, false);
	}

	private Vector2 ToVector2(Rect rect, TriangleNet.Data.Vertex vertex){
		return rect.position + new Vector2((float)vertex.X, rect.height - (float)vertex.Y) + Vector2.one * dotSize / 2;
	}

	private Vector2 ToVector2(Rect rect, Vector2 position){
		return rect.position + new Vector2(position.x, rect.height - position.y) + Vector2.one * dotSize / 2;
	}

	#endregion
}
}