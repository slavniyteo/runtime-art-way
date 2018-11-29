using System;
using System.Linq;
using System.Collections.Generic;
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
            sample.vertices.Add(position);

            return new SampleBuilder(sample);
        }

        public static SampleBuilder UpdateSample(Sample sample, Vector2 position)
        {
            Assert.IsFalse(sample.IsDrawn, "Sample must be empty");
            sample.vertices.Add(position);

            return new SampleBuilder(sample);
        }

        public static Sample CreateSample(IList<Vector2> line, float step)
        {
            var builder = CreateSample(line[0]);
            builder.Add(line.Skip(1));
            return builder.Build(step);
        }

        public static bool operator ==(SampleBuilder left, object right)
        {
            if ((object.ReferenceEquals(left, null) || left.isBuild)
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

        private Sample sample;
        private bool isBuild = false;

        private SampleBuilder(Sample sample)
        {
            this.sample = sample;
        }

        public void Add(Vector2 position)
        {
            sample.vertices.Add(position);
        }

        public void Add(IEnumerable<Vector2> positions)
        {
            sample.vertices.AddRange(positions);
        }

        public Sample Build(float step)
        {
            Assert.IsTrue(sample.vertices.Count > 1, "Sample must contain at least 2 dots");

            isBuild = true;

            sample.equalDistance = EqualDistanceUtil.Prepare(sample.vertices, step);
            sample.circuit = new CircuitCalculator().Calculate(sample.equalDistance, step);

            return sample;
        }
    }
}
#pragma warning restore 660,661
