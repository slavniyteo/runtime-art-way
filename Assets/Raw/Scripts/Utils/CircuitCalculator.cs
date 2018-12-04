using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

public class CircuitCalculator
{
    public List<Vector2> Calculate(List<Vector2> equalDistanceCloud, float step)
    {
        var result = FindCircuit(equalDistanceCloud, step * 1.501f);
        return result;
    }

    public List<Vector2> FindCircuit(List<Vector2> cloud, float radius)
    {
        int startPoint = FindStartPoint(cloud, radius);
        int secondPoint = startPoint == cloud.Count - 1 ? 0 : startPoint + 1;

        var cw = FindCircuitInDirection(cloud, startPoint, secondPoint, radius, true);
        var ccw = FindCircuitInDirection(cloud, startPoint, secondPoint, radius, false);

        if (cw.Count > ccw.Count)
        {
            return cw;
        }
        else
        {
            return ccw;
        }
    }

    private List<Vector2> FindCircuitInDirection(List<Vector2> cloud, int startPoint, int secondPoint, float radius,
        bool cw)
    {
        var points = cloud.Select(x => new Point(x)).ToList();
        points.RemoveAt(points.Count - 1);

        var result = new List<Vector2>();

        result.Add(points[startPoint].Position);
        points[startPoint].Enabled = false;

        result.Add(points[secondPoint].Position);
        points[secondPoint].Enabled = false;

        Vector2 previous = result[0];
        Vector2 current = result[1];
        Vector2 next;
        while (FindNext(points, previous, current, radius, cw, out next))
        {
            result.Add(next);
            previous = current;
            current = next;
        }

        result.Add(next);

        return result;
    }

    private int FindStartPoint(List<Vector2> cloud, float step)
    {
        var random = new Random();
        float min = cloud[0].y;
        float max = cloud[0].y;
        for (int i = 0; i < 10; i++)
        {
            min = Math.Min(min, cloud[random.Next() % cloud.Count].y);
            max = Math.Max(max, cloud[random.Next() % cloud.Count].y);
        }

        float average = (min + max) / 2;

        float radius = step * 2;
        int result = 0;
        float lastX = float.MaxValue;
        for (int i = 0; i < cloud.Count; i++)
        {
            Vector2 p = cloud[i];
            float distanceY = Math.Abs(p.y - average);
            if (distanceY < radius && lastX > p.x)
            {
                result = i;
                lastX = p.x;
            }
        }

        return result;
    }

    private static bool FindNext(List<Point> points, Vector2 previous, Vector2 current, float radius, bool cw,
        out Vector2 result)
    {
        var candidates = FindCandidates(points, previous, current, radius, cw).ToList();

        if (!candidates.Any())
        {
            Debug.LogError("Finish here with zero candidates");
            result = Vector2.zero;
            return false;
        }

        var next = SelectBestCandidate(candidates).Point;

        if (next.Enabled)
        {
            result = next.Position;
            next.Enabled = false;
            return true;
        }
        else
        {
            result = next.Position;
            next.Enabled = false;
            return false;
        }
    }

    private static Candidate SelectBestCandidate(IEnumerable<Candidate> candidates)
    {
        var sequence = new float[] {60, 120, 180, 240, 300, 360};

        candidates = candidates
            .OrderByDescending(c => c.Point.Enabled)
            .ThenBy(c => c.Angle)
            .ToList();

        IEnumerable<Candidate> matches = null;
        Candidate result = null;
        for (int i = 0; i < sequence.Length; i++)
        {
            float angle = sequence[i];
            matches = from c in candidates
                where c.Point.Enabled
                      && c.Angle > 30
                      && c.Angle <= angle
                select c;

            result = matches.FirstOrDefault();
            if (result != null)
            {
                return result;
            }
        }

        return candidates.First();
    }

    private static IEnumerable<Candidate> FindCandidates(IEnumerable<Point> points, Vector2 previous, Vector2 current,
        float radius, bool cw)
    {
        var prevDirection = current - previous;

        var result = from p in points
            where p.Position != previous && p.Position != current
            where (p.Position - current).magnitude <= radius
            let direction = p.Position - current
            let angle = 180 + (cw
                            ? Vector2.SignedAngle(prevDirection, direction)
                            : Vector2.SignedAngle(prevDirection, direction) * -1)
            select new Candidate(point: p, angle: angle);
        return result;
    }

    private class Point
    {
        public Vector2 Position { get; set; }
        public bool Enabled { get; set; }

        public Point(Vector2 position)
        {
            Position = position;
            Enabled = true;
        }

        public override string ToString()
        {
            return $"{Enabled}; {Position}";
        }
    }

    private class Candidate
    {
        public Point Point { get; private set; }
        public float Angle { get; private set; }

        public Candidate(Point point, float angle)
        {
            Point = point;
            Angle = angle;
        }

        public override string ToString()
        {
            return $"{Point} => {Angle}";
        }
    }
}