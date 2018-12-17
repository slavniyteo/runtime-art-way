using System;
using JetBrains.Annotations;
using UnityEngine;

namespace RuntimeArtWay
{
    public interface IRequestForShape
    {
        string Label { get; }
        UvAlgorithm UvAlgorithm { get; }
        Texture2D Texture { get; }
    }

    [Serializable]
    public class RequestForShape : IRequestForShape
    {
        [SerializeField] public UvAlgorithm uvAlgorithm = UvAlgorithm.Mask;
        public UvAlgorithm UvAlgorithm => uvAlgorithm;

        [SerializeField] public string label = "{{Placeholder}}";
        public string Label => label;

        [SerializeField] public Texture2D texture = null;
        public Texture2D Texture => texture;
    }
}