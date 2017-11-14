using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using System.Collections;

public class CircuitCalculatorTest {
	[Test, TestCaseSource(typeof(CalculateCircuitTestSource))]
	public void CalculateCircuitTest(string name, List<Vector2> line, float distance, List<Vector2> expected){
		var actual = new CircuitCalculator().Calculate(line, distance);
		actual.ForEach(x => Debug.Log(x));
		Assert.AreEqual(expected, actual);
	}

	[Test, TestCaseSource(typeof(CalculateCircuitTestSource))]
	public void CalculateCircuitTestCWW(string name, List<Vector2> line, float distance, List<Vector2> expected){
		line = Invert(line);

		var actual = new CircuitCalculator().Calculate(line, distance);
		actual.ForEach(x => Debug.LogFormat("Actual: {0}", x));

		expected = Invert(expected);
		expected.ForEach(x => Debug.LogFormat("Expected: {0}", x));
		Assert.AreEqual(expected, actual);
	}

	private List<Vector2> Invert(List<Vector2> list){
		var result = new List<Vector2>(list.Count);
		for (int i = 0; i < list.Count; i++){
			result.Add(list[list.Count - 1 - i]);
		}
		return result;
	}

    public class CalculateCircuitTestSource : IEnumerable {
        IEnumerator IEnumerable.GetEnumerator() {
			yield return new object[] {
				"Trivial",
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(6,5),
					new Vector2(6,6),
					new Vector2(5,6)
				},
				1,
				new List<Vector2>(){
					new Vector2(5,5),
					new Vector2(6,5),
					new Vector2(6,6),
					new Vector2(5,6),
				}
			};
			yield return new object[] {
				"Simple",
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(7,5),
					new Vector2(6.5f,6f),
					new Vector2(7,7),
					new Vector2(6,6.5f),
					new Vector2(5,7),
				},
				2,
				new List<Vector2>(){
					new Vector2(5,5),
					new Vector2(7,5),
					new Vector2(7,7),
					new Vector2(5,7),
				}
			};
			yield return new object[] {
				"Simple with propagate",
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(7,5),
					new Vector2(7,7),
					new Vector2(5,7),
				},
				1,
				new List<Vector2>(){
					new Vector2(5,5),
					new Vector2(6,5),
					new Vector2(7,5),
					new Vector2(7,6),
					new Vector2(7,7),
					new Vector2(6,7),
					new Vector2(5,7),
					new Vector2(5,6),
				}
			};
        }
    }


}
