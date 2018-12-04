using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using System.Collections;
using RuntimeArtWay.Circuit;

public class EqualDistanceUtilTest : BaseTest
{
    [Test, TestCaseSource(typeof(PrepareEqualDistancesSource))]
    public void PrepareEqualDistances(string name, List<Vector2> line, float distance, List<Vector2> expected)
    {
        var actual = EqualDistanceUtil.Prepare(line, distance);
        AssertEqual(expected, actual);
    }

    public class PrepareEqualDistancesSource : IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return new object[]
            {
                "Whole numbers",
                new List<Vector2>()
                {
                    new Vector2(5, 5),
                    new Vector2(5, 7)
                },
                1,
                new List<Vector2>()
                {
                    new Vector2(5, 5),
                    new Vector2(5, 6),
                    new Vector2(5, 7),
                    new Vector2(5, 6)
                }
            };
            yield return new object[]
            {
                "Little step",
                new List<Vector2>()
                {
                    new Vector2(5, 0.5f),
                    new Vector2(5, 0.7f)
                },
                0.1f,
                new List<Vector2>()
                {
                    new Vector2(5, 0.5f),
                    new Vector2(5, 0.6f),
                    new Vector2(5, 0.7f),
                    new Vector2(5, 0.6f),
                }
            };
            yield return new object[]
            {
                "Partial numbers",
                new List<Vector2>()
                {
                    new Vector2(5, 5),
                    new Vector2(5, 7.5f)
                },
                1,
                new List<Vector2>()
                {
                    new Vector2(5, 5),
                    new Vector2(5, 6),
                    new Vector2(5, 7),
                    new Vector2(5, 7.5f),
                    new Vector2(5, 6.5f),
                    new Vector2(5, 5.5f),
                }
            };
            yield return new object[]
            {
                "Partial numbers non 90 angle",
                new List<Vector2>()
                {
                    new Vector2(5, 5),
                    new Vector2(7.5f, 7.5f)
                },
                Vector2.one.magnitude,
                new List<Vector2>()
                {
                    new Vector2(5, 5),
                    new Vector2(6, 6),
                    new Vector2(7, 7),
                    new Vector2(7.5f, 7.5f),
                    new Vector2(6.5f, 6.5f),
                    new Vector2(5.5f, 5.5f),
                }
            };
            yield return new object[]
            {
                "Many redundant positions",
                new List<Vector2>()
                {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(0, 2),
                    new Vector2(0, 3),
                    new Vector2(0, 4),
                    new Vector2(0, 5),
                    new Vector2(0, 6),
                    new Vector2(0, 7),
                    new Vector2(0, 20)
                },
                10,
                new List<Vector2>()
                {
                    new Vector2(0, 0),
                    new Vector2(0, 10),
                    new Vector2(0, 20),
                    new Vector2(0, 10)
                }
            };
            yield return new object[]
            {
                "Many redundant positions with intermediate",
                new List<Vector2>()
                {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(0, 2),
                    new Vector2(0, 3),
                    new Vector2(0, 4),
                    new Vector2(0, 5),
                    new Vector2(0, 10),
                    new Vector2(0, 15),
                    new Vector2(0, 20)
                },
                10,
                new List<Vector2>()
                {
                    new Vector2(0, 0),
                    new Vector2(0, 10),
                    new Vector2(0, 20),
                    new Vector2(0, 10)
                }
            };
            yield return new object[]
            {
                "Live 1",
                new List<Vector2>()
                {
                    new Vector2(522.5266f, 452.3031f),
                    new Vector2(524.9855f, 448f),
                    new Vector2(524.9855f, 438),
                    new Vector2(519.4203f, 427.9542f),
                },
                10,
                new List<Vector2>()
                {
                }
            };
        }
    }
}