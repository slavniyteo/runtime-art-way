using System;

namespace RuntimeArtWay
{
    [Flags]
    public enum Layer
    {
        HandMade = 1,
        Propogated = 2,
        MeshSegments = 4,
        MeshCircuit = 8,
        MeshVerticles = 16,
        Texture = 32,
    }
}