using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriangleNet;
using UnityEngine;

namespace RuntimeArtWay
{
    [CreateAssetMenu]
    public class Sample : ScriptableObject
    {
        public List<Vector2> verticles = new List<Vector2>();
        public List<Vector2> equalDistance = new List<Vector2>();
        public List<Vector2> circuit = new List<Vector2>();

        public int Limit { get; set; }
        public int Count => Math.Max(verticles.Count, Math.Max(equalDistance.Count, circuit.Count));

        public List<Vector2> Verticles
        {
            get
            {
                if (Limit == 0 || Limit == verticles.Count) return verticles;
                int limit = Math.Min(Limit, verticles.Count);
                return verticles.GetRange(0, limit);
            }
        }

        public List<Vector2> EqualDistance
        {
            get
            {
                if (Limit == 0 || Limit == equalDistance.Count) return equalDistance;
                int limit = Math.Min(Limit, equalDistance.Count);
                return equalDistance.GetRange(0, limit);
            }
        }

        public List<Vector2> Circuit
        {
            get
            {
                if (Limit == 0 || Limit == circuit.Count) return circuit;
                int limit = Math.Min(Limit, circuit.Count);
                return circuit.GetRange(0, limit);
            }
        }

        public bool IsDrawn
        {
            get { return Verticles != null && Verticles.Count > 0; }
        }

        public bool IsPropagated
        {
            get { return EqualDistance != null && EqualDistance.Count > 0; }
        }

        public bool HasCircuit
        {
            get { return Circuit != null && Circuit.Count > 2; }
        }
    }
}