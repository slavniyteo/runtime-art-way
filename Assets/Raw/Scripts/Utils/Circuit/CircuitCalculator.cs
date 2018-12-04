using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace RuntimeArtWay.Circuit
{
    public class CircuitCalculator
    {
        public List<Vector2> Calculate(List<Vector2> equalDistanceCloud, float step)
        {
            return FindCircuit(equalDistanceCloud, step * 1.501f);
        }

        public List<Vector2> FindCircuit(List<Vector2> cloud, float radius)
        {
            int startPoint = FindStartPoint(cloud, radius);

            cloud.RemoveAt(cloud.Count - 1);
            var cw = FindCircuitInDirection(cloud, startPoint, radius, true);
            var ccw = FindCircuitInDirection(cloud, startPoint, radius, false);

            if (cw.Count > ccw.Count)
            {
                return cw;
            }
            else
            {
                return ccw;
            }
        }

        private static int FindStartPoint(List<Vector2> cloud, float step)
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

        private List<Vector2> FindCircuitInDirection(List<Vector2> cloud, int startPoint, float radius, bool cw)
        {
            var points = cloud.Select(x => new Point(x)).ToList();

            var result = new List<Vector2>();

            result.Add(points[startPoint].Position);
            points[startPoint].Enabled = false;

            int secondPoint = startPoint == cloud.Count - 1 ? 0 : startPoint + 1;
            result.Add(points[secondPoint].Position);
            points[secondPoint].Enabled = false;

            var seeker = new NextPointSeeker(points, radius, cw, result[0], result[1]);
            while (seeker.FindNext())
            {
                result.Add(seeker.Current);
            }

            result.Add(seeker.Last);

            return result;
        }
    }
}