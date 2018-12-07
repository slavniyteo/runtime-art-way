using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeArtWay
{
    public interface ISample
    {
        string name { get; set; }
        int Count { get; }

        float AverageStep { get; }

        List<Vector2> Vertices { get; }
        List<Vector2> EqualDistance { get; }
        List<Vector2> Circuit { get; }

        bool IsDrawn { get; }
        bool IsPropagated { get; }
        bool HasCircuit { get; }
    }

    public interface IEditableSample : ISample
    {
        List<Vector2> EqualDistance { get; set; }
        List<Vector2> Circuit { get; set; }

        void Add(Vector2 point);
        void AddRange(IEnumerable<Vector2> points);
    }

    [CreateAssetMenu]
    public class Sample : ScriptableObject, IEditableSample
    {
        public List<Vector2> vertices = new List<Vector2>();
        public List<Vector2> equalDistance = new List<Vector2>();
        public List<Vector2> circuit = new List<Vector2>();

        public int Count => Math.Max(vertices.Count, Math.Max(equalDistance.Count, circuit.Count));

        public List<Vector2> Vertices
        {
            get => vertices;
            set => vertices = value;
        }

        public List<Vector2> EqualDistance
        {
            get => equalDistance;
            set => equalDistance = value;
        }

        public List<Vector2> Circuit
        {
            get => circuit;
            set => circuit = value;
        }

        public void Add(Vector2 point)
        {
            vertices.Add(point);
        }

        public void AddRange(IEnumerable<Vector2> points)
        {
            vertices.AddRange(points);
        }

        public float AverageStep
        {
            get
            {
                var minStep = float.MaxValue;
                var maxStep = float.MinValue;
                float sum = 0;
                for (int i = 1; i < vertices.Count; i++)
                {
                    var step = (vertices[i] - vertices[i - 1]).magnitude;

                    if (step > 0)
                    {
                        minStep = Math.Min(minStep, step);
                    }

                    maxStep = Math.Max(maxStep, step);
                    sum += step;
                }

                return sum / vertices.Count;
            }
        }

        public bool IsDrawn => Vertices != null && Vertices.Count > 0;

        public bool IsPropagated => EqualDistance != null && EqualDistance.Count > 0;

        public bool HasCircuit => Circuit != null && Circuit.Count > 2;
    }
}