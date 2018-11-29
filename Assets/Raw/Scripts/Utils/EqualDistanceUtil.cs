using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EqualDistanceUtil
{
    public static List<Vector2> Prepare(List<Vector2> line, float step)
    {
        var result = new List<Vector2>();
        Vector2 last = line[0];
        result.Add(last);
        for (int i = 1; i < line.Count; i++)
        {
            if ((line[i] - last).magnitude < step)
            {
                continue;
            }

            foreach (var next in GetEqualDistances(last, line[i], step))
            {
                result.Add(next);
                last = next;
            }
        }

        result.Add(line[line.Count - 1]);
        result.AddRange(GetEqualDistances(line[line.Count - 1], line[0], step));

        return result;
    }

    private static IEnumerable<Vector2> GetEqualDistances(Vector2 from, Vector2 to, float stepValue)
    {
        var diff = to - from;
        var step = diff.normalized * stepValue;
        float stepsCount = diff.magnitude / step.magnitude;
        for (int i = 1; i < stepsCount; i++)
        {
            yield return from + step * i;
        }

        if (stepsCount - Math.Floor(stepsCount) < 0.5f)
        {
            yield return to;
        }
    }
}