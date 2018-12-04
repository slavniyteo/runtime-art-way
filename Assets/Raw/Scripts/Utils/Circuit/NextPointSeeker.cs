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

        public Vector2 Previous { get; private set; }
        public Vector2 Current { get; private set; }
        public Vector2 Last { get; private set; }

        public NextPointSeeker(List<Point> points, float radius, bool cw, Vector2 previous, Vector2 current)
        {
            this.points = new List<Point>(points);
            this.radius = radius;
            this.cw = cw;

            Previous = previous;
            Current = current;
        }

        public bool FindNext()
        {
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

        private Candidate SelectBestCandidate(IEnumerable<Candidate> candidates)
        {
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