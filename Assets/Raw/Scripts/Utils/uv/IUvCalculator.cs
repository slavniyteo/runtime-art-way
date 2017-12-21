using System.Collections.Generic;
using UnityEngine;

namespace RuntimeArtWay {
    public interface IUvCalculator {

        Vector2[] Calculate(TriangleNet.Mesh mesh);
        
    }
}