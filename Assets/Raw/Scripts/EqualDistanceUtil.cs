using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EqualDistanceUtil {
    public static List<Vector2> Prepare(List<Vector2> line, float step){
		var result = new List<Vector2>();
		Vector2 last = line[0];
		for (int i = 1; i < line.Count; i++){
			if ((line[i] - last).magnitude < step) {
                continue;
			}
            // Debug.LogFormat("Index: {0}, Result count: {1}", i, result.Count);
            result.AddRange(GetEqualDistances(last, line[i], step));
            last = line[i];
            // Debug.LogFormat("Index: {0}, Position: {1}, Result count: {2}, Last: {3}", i, line[i], result.Count, last);
		}
		result.AddRange(GetEqualDistances(line[line.Count - 1], line[0], step));

        // for (int i = 1; i < result.Count; i++){
        //     if ((result[i] - result[i - 1]).magnitude > step){
        //         result.ForEach(x => Debug.Log(x));
        //         Debug.Log("Fail at " + i);
        //         throw new Exception();
        //     }
        // }
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
