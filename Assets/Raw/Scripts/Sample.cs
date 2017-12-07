using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriangleNet;
using UnityEngine;

namespace RuntimeArtWay {

[CreateAssetMenu]
public class Sample : ScriptableObject {

    public Vector2[] verticles = new Vector2[0];
    public Vector2[] equalDistance = new Vector2[0];
    public Vector2[] circuit = new Vector2[0];


    public bool IsDrawn { 
        get { 
            return verticles != null 
                    && verticles.Length > 0
                    && equalDistance != null 
                    && equalDistance.Length > 0;
        } 
    }
    public bool HasCircuit { 
        get { 
            return circuit != null && circuit.Length > 0; 
        } 
    }
}
}