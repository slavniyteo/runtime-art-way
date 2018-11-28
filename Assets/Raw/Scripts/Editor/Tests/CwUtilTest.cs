using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using System.Collections;

public class CwUtilTest : BaseTest
{
    [Test, TestCaseSource(typeof(IsLineClockWiseSource))]
    public void IsLineClockWise(string name, List<Vector2> line, bool expected)
    {
        var actual = CwUtil.IsLineClockWise(line);
        Assert.AreEqual(expected, actual);
    }

    public class IsLineClockWiseSource : IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return new object[]
            {
                "Whole numbers",
                new List<Vector2>()
                {
                    new Vector2(5, 5),
                    new Vector2(5, 7),
                    new Vector2(4, 6),
                },
                true
            };
            yield return new object[]
            {
                "Whole numbers",
                new List<Vector2>()
                {
                    new Vector2(5, 5),
                    new Vector2(5, 7),
                    new Vector2(6, 6),
                },
                false
            };
        }
    }
}