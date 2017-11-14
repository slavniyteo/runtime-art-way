using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EqualDistanceUtil {
    public static List<Vector2> Prepare(List<Vector2> line, float step){
		var result = new List<Vector2>();
		for (int i = 0; i < line.Count - 1; i++){
			result.AddRange(GetEqualDistances(line[i], line[i+1], step));
		}
		result.AddRange(GetEqualDistances(line[line.Count - 1], line[0], step));
		return result;
	}

    private static IEnumerable<Vector2> GetEqualDistances(Vector2 from, Vector2 to, float stepValue) {
		var diff = to - from;
		var step = diff.normalized * stepValue;
		var stepsCount = diff.magnitude / step.magnitude;
		for (int i = 0; i < stepsCount; i++){
			yield return from + step*i;
		}
    }
}
