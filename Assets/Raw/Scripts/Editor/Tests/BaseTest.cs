using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;

public class BaseTest
{
    public void AssertEqual(IEnumerable<Vector2> expected, IEnumerable<Vector2> actual, string message = "")
    {
        if (expected.Count() != actual.Count())
        {
            foreach (var ex in expected) Debug.Log("Expected: " + ex);
            foreach (var ac in actual) Debug.Log("Actual: " + ac);
            Assert.Fail(message + "\nDifferent sizes: expected is {0}, actual is {1}", expected.Count(),
                actual.Count());
        }

        var expectedArray = expected.ToArray();
        var actualArray = actual.ToArray();
        for (int i = 0; i < expectedArray.Length; i++)
        {
            if (expectedArray[i] != actualArray[i])
            {
                foreach (var ex in expected) Debug.Log("Expected: " + ex);
                foreach (var ac in actual) Debug.Log("Actual: " + ac);
                Assert.Fail(message + "\nElement {0} is bad: \nExpected: {1} \nActual: {2}", i, expectedArray[i],
                    actualArray[i]);
            }
        }
    }
}