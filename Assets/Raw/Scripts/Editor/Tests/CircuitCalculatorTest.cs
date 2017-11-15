using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using System.Collections;

public class CircuitCalculatorTest {
	[Test, TestCaseSource(typeof(CalculateCircuitTestSource))]
	public void CalculateCircuitTest(string name, List<Vector2> line, float distance, List<Vector2> expected){
		var actual = new CircuitCalculator().Calculate(ref line, distance);
		actual.ForEach(x => Debug.Log(x));
		Assert.AreEqual(expected, actual);
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

	[Test, TestCaseSource(typeof(CalculateCircuitTestCwwSource))]
	public void CalculateCircuitTestCWW(string name, List<Vector2> line, float distance, List<Vector2> expected){
		var actual = new CircuitCalculator().Calculate(ref line, distance);
		Assert.AreEqual(expected, actual);
	}

    public class CalculateCircuitTestCwwSource : IEnumerable {
        IEnumerator IEnumerable.GetEnumerator() {
			yield return new object[] {
				"Trivial",
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(5,6),
					new Vector2(6,6),
					new Vector2(6,5),
				},
				1,
				new List<Vector2>(){
					new Vector2(5,5),
					new Vector2(5,6),
					new Vector2(6,6),
					new Vector2(6,5),
				}
			};
			yield return new object[] {
				"Simple",
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(5,7),
					new Vector2(6.5f,6f),
					new Vector2(7,7),
					new Vector2(6,6.5f),
					new Vector2(7,5),
				},
				2,
				new List<Vector2>(){
					new Vector2(5,5),
					new Vector2(5,7),
					new Vector2(7,7),
					new Vector2(7,5),
				}
			};
			yield return new object[] {
				"Simple with propagate",
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(5,7),
					new Vector2(7,7),
					new Vector2(7,5),
				},
				1f,
				new List<Vector2>(){
					new Vector2(5,5),
					new Vector2(5,6),
					new Vector2(5,7),
					new Vector2(6,7),
					new Vector2(7,7),
					new Vector2(7,6),
					new Vector2(7,5),
					new Vector2(6,5),
				}
			};
        }
    }

}
