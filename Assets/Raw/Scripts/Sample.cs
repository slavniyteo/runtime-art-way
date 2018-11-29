using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeArtWay
{
    [CreateAssetMenu]
    public class Sample : ScriptableObject
    {
        public List<Vector2> vertices = new List<Vector2>();
        public List<Vector2> equalDistance = new List<Vector2>();
        public List<Vector2> circuit = new List<Vector2>();

        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
        public int Count => Math.Max(vertices.Count, Math.Max(equalDistance.Count, circuit.Count));

        public List<Vector2> Vertices => vertices;
        public List<Vector2> EqualDistance => CutList(equalDistance);
        public List<Vector2> Circuit => CutList(circuit);

        private List<Vector2> CutList(List<Vector2> list)
        {
            int fromIndex = GetFromIndex(list.Count);
            int toIndex = GetToIndex(list.Count);

            if (fromIndex == 0 && toIndex == 0) return list;
            return list.GetRange(fromIndex, toIndex - fromIndex + 1);
        }

        private int GetFromIndex(int maxValue)
        {
            return Math.Max(0, Math.Min(ToIndex, Math.Min(FromIndex, maxValue - 1)));
        }

        private int GetToIndex(int maxValue)
        {
            return Math.Max(0, Math.Min(ToIndex, maxValue - 1));
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