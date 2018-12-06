using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace RuntimeArtWay.Circuit
{
    public class CircuitCalculator
    {
        public List<Vector2> Calculate(List<Vector2> equalDistanceCloud, float step)
        {
            return FindCircuit(equalDistanceCloud, step * 1.401f);
        }

        private List<Vector2> FindCircuit(List<Vector2> cloud, float radius)
        {
            int startPoint = FindStartPoint(cloud, radius);

            cloud.RemoveAt(cloud.Count - 1);
            var cw = FindCircuitInDirection(cloud, startPoint, radius, true);
            var ccw = FindCircuitInDirection(cloud, startPoint, radius, false);

            return cw.Count > ccw.Count ? cw : ccw;
        }

        private static int FindStartPoint(List<Vector2> cloud, float step)
        {
            int result = 0;
            float minX = float.MaxValue;
            for (int i = 0; i < cloud.Count; i++)
            {
                Vector2 p = cloud[i];
                if (p.x < minX)
                {
                    result = i;
                    minX = p.x;
                }
            }

            return result;
        }

        private List<Vector2> FindCircuitInDirection(List<Vector2> cloud, int startPoint, float radius, bool cw)
        {
            var points = cloud.Select(x => new Point(x)).ToList();

            var result = new List<Vector2>();

            var seeker = new NextPointSeeker(points, radius, cw, startPoint);
            result.Add(seeker.First);

            while (seeker.FindNext())
            {
                result.Add(seeker.Current);
            }

            result.Add(seeker.Last);

            return result;
        }
    }
}