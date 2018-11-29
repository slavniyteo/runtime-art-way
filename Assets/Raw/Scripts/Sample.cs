using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriangleNet;
using UnityEditor;
using UnityEngine;

namespace RuntimeArtWay
{
    [CreateAssetMenu]
    public class Sample : ScriptableObject
    {
        public List<Vector2> vertices = new List<Vector2>();
        public List<Vector2> equalDistance = new List<Vector2>();
        public List<Vector2> circuit = new List<Vector2>();

        public int Offset { get; set; }
        public int Limit { get; set; }
        public int Count => Math.Max(vertices.Count, Math.Max(equalDistance.Count, circuit.Count));

        public List<Vector2> Vertices => vertices; 
        public List<Vector2> EqualDistance => CutList(equalDistance);
        public List<Vector2> Circuit => CutList(circuit);

        private List<Vector2> CutList(List<Vector2> list)
        {
            int offset = GetOffset(list.Count);
            int limit = GetLimit(list.Count);

            if (offset == 0 && limit == 0) return list;
            return list.GetRange(offset, limit - offset);
        }

        private int GetOffset(int maxValue)
        {
            return Math.Max(0, Math.Min(Limit, Math.Min(Offset, maxValue - 1)));
        }

        private int GetLimit(int maxValue)
        {
            return Math.Max(0, Math.Min(Limit, maxValue - 1));
        }


        public bool IsDrawn
        {
            get { return Vertices != null && Vertices.Count > 0; }
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