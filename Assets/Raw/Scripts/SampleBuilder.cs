using System;
using System.Linq;
using System.Collections.Generic;
using RuntimeArtWay.Circuit;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 660,661
namespace RuntimeArtWay
{
    public class SampleBuilder
    {
        public static SampleBuilder CreateSample(Vector2 position)
        {
            var sample = ScriptableObject.CreateInstance<Sample>();
            sample.Add(position);

            return new SampleBuilder(sample);
        }

        public static SampleBuilder UpdateSample(IEditableSample sample, Vector2 position)
        {
            Assert.IsFalse(sample.IsDrawn, "Sample must be empty");
            sample.Add(position);

            return new SampleBuilder(sample);
        }

        public static ISample CreateSample(IList<Vector2> line, float step)
        {
            var builder = CreateSample(line[0]);
            builder.Add(line.Skip(1));
            return builder.Build(step);
        }

        public static bool operator ==(SampleBuilder left, object right)
        {
            if ((object.ReferenceEquals(left, null) || left.isBuilt)
                && object.ReferenceEquals(right, null))
            {
                return true;
            }

            return object.ReferenceEquals(left, right);
        }

        public static bool operator !=(SampleBuilder left, object right)
        {
            return !(left == right);
        }

        private IEditableSample sample;
        private bool isBuilt = false;

        private SampleBuilder(IEditableSample sample)
        {
            this.sample = sample;
        }

        public void Add(Vector2 position)
        {
            sample.Add(position);
        }

        public void Add(IEnumerable<Vector2> positions)
        {
            sample.AddRange(positions);
        }

        public ISample Build(float stepMultiplier)
        {
            Assert.IsTrue(sample.Count > 1, "Sample must contain at least 2 dots");

            isBuilt = true;

            Rebuild(sample, stepMultiplier);

            return sample;
        }

        public static void Rebuild(IEditableSample sample, float stepMultiplier)
        {
            var startTime = DateTime.Now;

            float step = sample.AverageStep * stepMultiplier;
            sample.EqualDistance = EqualDistanceUtil.Prepare(sample.Vertices, step);
            sample.Circuit = new CircuitCalculator().Calculate(sample.EqualDistance, step);

            var endTime = DateTime.Now;
            Debug.Log($"Rebuild sample in {(endTime - startTime).TotalMilliseconds} ms");
        }
    }
}
#pragma warning restore 660,661