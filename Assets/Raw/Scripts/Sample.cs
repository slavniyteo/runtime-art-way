using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriangleNet;
using UnityEngine;

namespace RuntimeArtWay {

[CreateAssetMenu]
public class Sample : ScriptableObject {

    public List<Vector2> verticles = new List<Vector2>();
    public List<Vector2> equalDistance = new List<Vector2>();
    public List<Vector2> circuit = new List<Vector2>();


    public bool IsDrawn { 
        get { return verticles != null && verticles.Count > 0; } 
    }

    public bool IsPropagated {
        get { return equalDistance != null && equalDistance.Count > 0; }
    }
    public bool HasCircuit { 
        get { return circuit != null && circuit.Count > 0; } 
    }
}
}