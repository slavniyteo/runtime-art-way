using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using System.Collections;

public class CircuitCalculatorTest : BaseTest {
	[Test, TestCaseSource(typeof(CalculateCircuitTestSource))]
	public void CalculateCircuitTest(string name, List<Vector2> line, float distance, List<Vector2> expected){
		var actual = new CircuitCalculator().FindCircuit(line, distance, true);
		AssertEqual(expected, actual);
	}

    public class CalculateCircuitTestSource : IEnumerable {
        IEnumerator IEnumerable.GetEnumerator() {
			yield return new object[] {
				"Trivial",
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(5,6),
					new Vector2(6,5),
				},
				1.5f,
				new List<Vector2>(){
					new Vector2(5,5),
					new Vector2(5,6),
					new Vector2(6,5),
					new Vector2(5,5),
				}
			};
			yield return new object[] {
				"Simple",
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(6,5),
					new Vector2(7,6),
					new Vector2(6,6),
					new Vector2(5,6),
				},
				2,
				new List<Vector2>(){
					new Vector2(5,5),
					new Vector2(6,5),
					new Vector2(7,6),
					new Vector2(6,6),
					new Vector2(5,6),
					new Vector2(5,5),
				}
			};
			yield return new object[] {
				"Simple with propagate",
				new List<Vector2>() {
					new Vector2(4,6),
					new Vector2(5,7),
					new Vector2(5.9f,6),
					new Vector2(7,5),
					new Vector2(8,6),
					new Vector2(7,7),
					new Vector2(6.1f,6),
					new Vector2(5,5),
				},
				Vector2.one.magnitude*1.3f,
				new List<Vector2>(){
					new Vector2(4,6),
					new Vector2(5,7),
					new Vector2(5,5),
					new Vector2(6.1f,6),
					new Vector2(7,7),
					new Vector2(8,6),
					new Vector2(7,5),
					new Vector2(5.9f,6),
					new Vector2(5,5),
					new Vector2(4,6),
				}
			};
        }
    }

	[Test, TestCaseSource(typeof(CalculateCircuitTestCwwSource))]
	public void CalculateCircuitTestCWW(string name, List<Vector2> line, float distance, List<Vector2> expected){
		line = EqualDistanceUtil.Prepare(line, distance);
		var actual = new CircuitCalculator().Calculate(line, distance);
		AssertEqual(expected, actual);
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
