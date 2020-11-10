using System;

namespace RuntimeArtWay
{
    [Flags]
    public enum Layer
    {
        HandMade = 1 << 0,
        Propogated = 1 << 1,
        SubSample = 1 << 2,
        Graph = 1 << 3,
        MeshSegments = 1 << 4,
        MeshCircuit = 1 << 5,
        MeshVerticles = 1 << 6,
        Texture = 1 << 7,
    }
}