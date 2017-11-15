using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriangleNet;
using UnityEngine;

[CreateAssetMenu]
public class TriangulatePreview : ScriptableObject {

	public Vector2[] verticles;
	public Vector2[] equalDistance;
	public Vector2[] circuit;

	public TriangleNet.Mesh GenerateMesh(Rect bounds) {
		var verticles = NormilizedVerticles(this.verticles, bounds);

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

}
