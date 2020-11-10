using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeArtWay
{
    public class PreviewSample : ISample
    {
        private readonly ISample origin;

        public PreviewSample(ISample origin)
        {
            this.origin = origin;
        }

        public int FromIndex { get; set; }
        public int ToIndex { get; set; }

        public List<Vector2> EqualDistance => CutList(origin.EqualDistance);

        public List<Vector2> Circuit => CutList(origin.Circuit);

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

        #region Delegate

        public string name
        {
            get => origin.name;
            set => origin.name = value;
        }

        public int Count => origin.Count;

        public float AverageStep => origin.AverageStep;
        public float EqualDistanceStep => origin.EqualDistanceStep;

        public List<Vector2> Vertices => origin.Vertices;

        public bool IsDrawn => origin.IsDrawn;

        public bool IsPropagated => origin.IsPropagated;

        public bool HasCircuit => origin.HasCircuit;

        #endregion
    }
}