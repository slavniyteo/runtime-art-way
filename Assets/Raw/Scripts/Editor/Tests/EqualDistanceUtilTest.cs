using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using System.Collections;

public class EqualDistanceUtilTest {

	[Test, TestCaseSource(typeof(PrepareEqualDistancesSource))]
	public void PrepareEqualDistances(string name, List<Vector2> line, float distance, List<Vector2> expected){
		var actual = EqualDistanceUtil.Prepare(line, distance);
		Assert.AreEqual(expected, actual);
	}

    public class PrepareEqualDistancesSource : IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator() {
			yield return new object[] {
				"Whole numbers",
				new List<Vector2>() {
					new Vector2(5, 5),
					new Vector2(5,7)
				},
				1,
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(5,6),
					new Vector2(5,7),
					new Vector2(5,6)

				}
			};
			yield return new object[] {
				"Partial numbers",
				new List<Vector2>() {
					new Vector2(5, 5),
					new Vector2(5,7.5f)
				},
				1,
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(5,6),
					new Vector2(5,7),
					new Vector2(5,7.5f),
					new Vector2(5,6.5f),
					new Vector2(5,5.5f),
				}
			};
			yield return new object[] {
				"Partial numbers non 90 angle",
				new List<Vector2>() {
					new Vector2(5, 5),
					new Vector2(7.5f,7.5f)
				},
				Vector2.one.magnitude,
				new List<Vector2>() {
					new Vector2(5,5),
					new Vector2(6,6),
					new Vector2(7,7),
					new Vector2(7.5f,7.5f),
					new Vector2(6.5f,6.5f),
					new Vector2(5.5f,5.5f),
				}
			};
        }
    }

}
