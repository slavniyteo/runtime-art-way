﻿using System;
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

	private Drawer drawer;

	private ILayers layers;
	private bool drawFullPreview = false;

	private float dotSize;

	public Preview(ILayers layers, float dotSize = 5) {
		drawer = new Drawer();

		this.layers = layers;
		layers.onChange += OnLayersChange;

		this.dotSize = dotSize;
	}

	private void OnLayersChange(Layer oldValue, Layer newValue){
	}

	protected override void OnShow() {
		drawer.Show(target);
	}

	protected override void OnHide(){
		drawer.Hide();
	}

	#region Draw Mesh Preview

	protected override void OnDraw(){
		var rect = GUILayoutUtility.GetAspectRect(1);
		drawer.Draw(rect);
		StatelessDraw(rect, target);
	}

	public void StatelessDraw(Rect rect, Sample target){
		EditorGUI.DrawRect(rect, Color.gray);

		if (! target.IsDrawn) return;

		var factor = Factor(rect, target.verticles);

		if ((layers.Value & Layer.HandMade) == Layer.HandMade){
			var verticles = NormilizedVerticles(target.verticles, factor);
			DrawDots(rect, verticles, Color.red);
		}

		if (! target.IsPropagated) return; 

		if ((layers.Value & Layer.Propogated) == Layer.Propogated){
			var equalDistance = NormilizedVerticles(target.equalDistance, factor);
			DrawDots(rect, equalDistance, Color.green);
		}

		if (! target.HasCircuit) return;

		var meshCircuit = NormilizedVerticles(target.circuit, factor);
		var mesh = MeshGenerator.Generate(meshCircuit);

		if ((layers.Value & Layer.MeshSegments) == Layer.MeshSegments){
			DrawTriangles(rect, mesh);
		}
		if ((layers.Value & Layer.MeshCircuit ) == Layer.MeshCircuit){
			var circuit = NormilizedVerticles(target.circuit, factor);
			DrawLine(rect, circuit, Color.magenta);
		}
		if ((layers.Value & Layer.MeshVerticles) == Layer.MeshVerticles){
			DrawVerticles(rect, mesh);
		}
	}

	private void DrawDots(Rect rect, IList<Vector2> line, Color color){
		for (int i = 1; i < line.Count; i++){
			DrawPoint(rect, ToVector2(rect, line[i]), color);
		}
	}

	private void DrawLine(Rect rect, List<Vector2> line, Color color){
		for (int i = 1; i < line.Count; i++){
			DrawLine(rect, ToVector2(rect, line[i - 1]), ToVector2(rect, line[i]), color);
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

	private Func<Vector2, Vector2> Factor(Rect rect, Vector2[] verticles){
		var min = verticles.Aggregate((v, res) => new Vector2(Mathf.Min(v.x, res.x), Mathf.Min(v.y, res.y)));
		var max = verticles.Aggregate((v, res) => new Vector2(Mathf.Max(v.x, res.x), Mathf.Max(v.y, res.y)));
		max = max - min;

		return (pos) => {
			Vector2 v = pos - min;
			return new Vector2(
				x: (v.x / max.x) * (rect.width - dotSize * 4) + dotSize * 2,
				y: (v.y / max.y) * (rect.height - dotSize * 4) + dotSize * 2
			);
		};
	}

	private List<Vector2> NormilizedVerticles(Vector2[] verticles, Func<Vector2, Vector2> factor){
		return verticles.Select(factor).ToList();
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