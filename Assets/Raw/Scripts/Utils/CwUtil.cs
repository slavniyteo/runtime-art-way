using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CwUtil
{
    public static List<Vector2> Prepare(List<Vector2> line)
    {
        if (IsLineClockWise(line))
        {
            return line;
        }
        else
        {
            var result = new List<Vector2>(line.Count);
            for (int i = 0; i < line.Count; i++)
            {
                result.Add(line[line.Count - 1 - i]);
            }

            return result;
        }
    }

    public static bool IsLineClockWise(List<Vector2> line)
    {
        var centerDirection = line.Aggregate((x, sum) => sum += x) / line.Count - line[0];
        var startDirection = line[1] - line[0];
        var angle = Vector2.SignedAngle(centerDirection, startDirection);
        Debug.Log($"To center: {centerDirection}, start: {startDirection}, angle: {angle}");
        return angle < 0;
    }
}