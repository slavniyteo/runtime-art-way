using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircuitCalculator {

	public List<Vector2> Calculate(ref List<Vector2> cloud, float step){
		int count = cloud.Count;
		cloud = EqualDistanceUtil.Prepare(cloud, step);
		var cw = CwUtil.IsLineClockWise(cloud);
		var result = FindCircuit(cloud, step*1.001f, cw);
		return result;
	}

    private List<Vector2> FindCircuit(List<Vector2> cloud, float radius, bool cw) {
		Debug.Log("===============================================");
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

		var candidates = FindCandidates(points, previous, current, radius, cw);
		if (candidates.Where(x => x.Point.Enabled).Count() == 0){
			Debug.Log("Candidates count = 0. Find far away");
			candidates = FindCandidates(points, previous, current, radius*1.5f, cw);
		}
		var next = candidates.First().Point;

		if (next.Enabled) {
			result = next.Position;
			next.Enabled = false;
			return true;
		}
		else {
			DebugLog(points, current, candidates);

			result = Vector2.zero;
			return false;
		}
	}

	private static IEnumerable<Candidate> FindCandidates(IEnumerable<Point> points, Vector2 previous, Vector2 current, float radius, bool cw){
		var prevDirection = current - previous;

		var result = from p in points
					where p.Position != previous && p.Position != current
					// where p.Enabled
					where (p.Position - current).magnitude <= radius
					let direction = p.Position - current
					let angle = cw ? Vector2.SignedAngle(prevDirection, direction) : Vector2.SignedAngle(prevDirection, direction) * -1
					where angle > -170 && angle < 170
					orderby angle ascending
					select new Candidate(point: p, angle: angle);
		return result;
	}

	private static void DebugLog(List<Point> points, Vector2 current, IEnumerable<Candidate> candidates){
			candidates.ToList().ForEach(x => Debug.LogFormat("Position: {0}, angle: {1}, Enabled: {2}", x.Point.Position, x.Angle, x.Point.Enabled));
			var index = points.FindIndex(x => x.Position == current);
			var message = "Points: ";
			for (int i = Mathf.Max(0, index - 5); i < Mathf.Min(points.Count, index + 5); i++){
				message += $"({points[i].Position}, {points[i].Enabled}),";
			}
			Debug.Log(message);
	}

	private class Point {
		public Vector2 Position { get; set; }
		public bool Enabled { get; set; }

		public Point(Vector2 position){
			Position = position;
			Enabled = true;
		}

	}

	private class Candidate {
		public Point Point { get; private set; }
		public float Angle { get; private set; }

		public Candidate(Point point, float angle){
			Point = point;
			Angle = angle;
		}
	}
}
