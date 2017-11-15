using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircuitCalculator {

	public List<Vector2> Calculate(ref List<Vector2> cloud, float step){
		int count = cloud.Count;
		cloud = EqualDistanceUtil.Prepare(cloud, step);
		var cw = CwUtil.IsLineClockWise(cloud);
		var result = FindCircuit(cloud, step*1.1f, cw);
		return result;
	}

    private List<Vector2> FindCircuit(List<Vector2> cloud, float radius, bool cw) {
		var result = new List<Vector2>();

		var points = cloud.Select(x => new Point(x)).ToList();
		points[0].Enabled = false;
		result.Add(points[0].Position);
		points[1].Enabled = false;
		result.Add(points[1].Position);

		Vector2 previous = result[0];
		Vector2 current = result[1];
		Vector2 next = Vector2.zero;
		while (FindNext(points, previous, current, radius, cw, out next)){
			result.Add(next);
			previous = current;
			current = next;
		}

		return result;
    }

	private bool FindNext(List<Point> points, Vector2 previous, Vector2 current, float radius, bool cw, out Vector2 result){
		var prevDirection = current - previous;

		var candidates = from p in points
					where p.Position != previous && p.Position != current
					where (p.Position - current).magnitude <= radius
					let direction = p.Position - current
					let angle = cw ? Vector2.SignedAngle(prevDirection, direction) : Vector2.SignedAngle(prevDirection, direction) * -1
					where angle > -180 && angle < 180
					orderby angle ascending
					select new {point = p, angle = angle};

		// points.ForEach(x => Debug.LogFormat("radius: {0}, magnitude: {1}", radius, (x.Position - current).magnitude));

		// Debug.LogFormat("From previous: {0}", prevDirection);
		// Debug.LogFormat("\nPrevious: {0}, Current: {1}", previous, current);
		// candidates.ToList().ForEach(x => Debug.LogFormat("Position: {0}, angle: {1}, Enabled: {2}", x.point.Position, x.angle, x.point.Enabled));

		var next = candidates.First().point;
		if (next.Enabled) {
			result = next.Position;
			next.Enabled = false;
			return true;
		}
		else {
			result = Vector2.zero;
			return false;
		}
	}

	private class Point {
		public Vector2 Position { get; set; }
		public bool Enabled { get; set; }

		public Point(Vector2 position){
			Position = position;
			Enabled = true;
		}

	}
}
