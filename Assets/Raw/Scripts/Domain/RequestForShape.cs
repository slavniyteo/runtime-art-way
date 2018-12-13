using System;
using JetBrains.Annotations;
using UnityEngine;

namespace RuntimeArtWay
{
    public interface IRequestForShape
    {
        string Label { get; }
        UvAlgorithm UvAlgorithm { get; }
    }

    [Serializable]
    public class RequestForShape : IRequestForShape
    {
        [SerializeField] private UvAlgorithm uvAlgorithm = UvAlgorithm.Mask;
        public UvAlgorithm UvAlgorithm => uvAlgorithm;

        [NotNull] [SerializeField] private string label = "{{Placeholder}}";
        public string Label => label;
    }
}