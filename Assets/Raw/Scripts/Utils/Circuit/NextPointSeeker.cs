using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeArtWay.Circuit
{
    internal class NextPointSeeker
    {
        private const float MIN_ANGLE = 30;

        private readonly List<Point> points;
        private readonly float radius;
        private readonly bool cw;

        public Vector2 First { get; }
        public Vector2 Previous { get; private set; }
        public Vector2 Current { get; private set; }
        public Vector2 Last { get; private set; }

        private int iteration;

        public NextPointSeeker(List<Point> points, float radius, bool cw, int startPointIndex)
        {
            this.points = new List<Point>(points);
            this.radius = radius;
            this.cw = cw;

            var startPoint = points[startPointIndex];
            First = startPoint.Position;
            startPoint.Enabled = false;

            Previous = cw ? First + new Vector2(-0.01f, 1f) : First + new Vector2(-0.01f, -1f);
            Current = First;
        }

        public bool FindNext()
        {
            iteration += 1;
            var candidates = FindCandidates(radius).ToList();
            var next = SelectBestCandidate(candidates).Point;

            if (!next.Enabled && next.Position != First)
            {
                candidates = FindCandidates(radius * 3).ToList();
                next = SelectBestCandidate(candidates).Point;
            }

            Previous = Current;
            Current = next.Position;

            if (next.Enabled)
            {
                next.Enabled = false;
                return true;
            }
            else
            {
                Last = Current;
                return false;
            }
        }

        private IEnumerable<Candidate> FindCandidates(float radius)
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
            if (candidates.Count == 0)
            {
                Debug.LogError("Finish here with zero candidates");
                return new Candidate(new Point(Vector2.zero, false), 0);
            }

            if (iteration > 5)
            {
                var startPointCandidates = candidates.FirstOrDefault(c => c.Point.Position == First);
                if (startPointCandidates != null)
                {
                    return startPointCandidates;
                }
            }

            candidates = candidates
                .OrderByDescending(c => c.Point.Enabled)
                .ThenBy(c => c.Angle)
                .ToList();

            var result = candidates
                .FirstOrDefault(c => c.Point.Enabled
                                     && c.Angle > MIN_ANGLE);
            return result ?? candidates.First();
        }
    }
}