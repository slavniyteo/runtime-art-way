using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeArtWay.Circuit
{
    internal class NextPointSeeker
    {
        private const float MIN_ANGLE = 30;
        private static readonly float[] ANGLES_SEQUENCE = new float[] {60, 120, 180, 240, 300, 360};


        private readonly List<Point> points;
        private readonly float radius;
        private readonly bool cw;

        public Vector2 First { get; }
        public Vector2 Previous { get; private set; }
        public Vector2 Current { get; private set; }
        public Vector2 Last { get; private set; }

        private int iteration = 0;

        public NextPointSeeker(List<Point> points, float radius, bool cw, int startPointIndex)
        {
            this.points = new List<Point>(points);
            this.radius = radius;
            this.cw = cw;

            var startPoint = points[startPointIndex];
            First = startPoint.Position;
            startPoint.Enabled = false;

            Previous = cw ? Vector2.down : Vector2.up;
            Current = First;
        }

        public bool FindNext()
        {
            iteration += 1;
            var candidates = FindCandidates().ToList();

            if (!candidates.Any())
            {
                Debug.LogError("Finish here with zero candidates");
                Previous = Current;
                Current = Vector2.zero;
                Last = Current;
                return false;
            }

            var next = SelectBestCandidate(candidates).Point;
            Previous = Current;
            Current = next.Position;

            if (next.Enabled)
            {
                next.Enabled = false;
                return true;
            }
            else
            {
                next.Enabled = false;
                Last = Current;
                return false;
            }
        }

        private IEnumerable<Candidate> FindCandidates()
        {
            var prevDirection = Current - Previous;

            var result = from p in points
                where p.Position != Previous
                      && p.Position != Current
                      && (p.Position - Current).magnitude <= radius
                let direction = p.Position - Current
                let angle = 180 + (cw
                                ? Vector2.SignedAngle(prevDirection, direction)
                                : (Vector2.SignedAngle(prevDirection, direction) * -1))
                select new Candidate(point: p, angle: angle);
            return result;
        }

        private Candidate SelectBestCandidate(List<Candidate> candidates)
        {
            if (iteration > 5)
            {
                var startPointCandidates = candidates.FirstOrDefault(c => c.Point.Position == First);
                if (startPointCandidates != null)
                {
                    Debug.Log($"Found first point at {iteration}! [{startPointCandidates}]; First: {First}");
                    return startPointCandidates;
                }
            }

            candidates = candidates
                .OrderByDescending(c => c.Point.Enabled)
                .ThenBy(c => c.Angle)
                .ToList();

            foreach (var angle in ANGLES_SEQUENCE)
            {
                var result = candidates.FirstOrDefault(c => c.Point.Enabled
                                                            && c.Angle > MIN_ANGLE
                                                            && c.Angle <= angle);
                if (result != null)
                {
                    return result;
                }
            }

            return candidates.First();
        }
    }
}