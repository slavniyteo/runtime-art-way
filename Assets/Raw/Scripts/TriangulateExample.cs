using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriangulateExample : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

    public TriangulatePreview preview;
    public LineRenderer line;
    public LineRenderer circuit;
    public LineRenderer equalDistance;

    public float step = 10;

    private CircuitCalculator circuitCalculator = new CircuitCalculator();

    private List<Vector2> verticles;

    void Start () {
        verticles = new List<Vector2>();
    }
    
    void Update () {
        if (line.positionCount != verticles.Count){
            line.positionCount = verticles.Count;
            line.SetPositions(verticles.Select(x => (Vector3) x).ToArray());
        }
        
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        verticles.Clear();
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        verticles.Add(eventData.position);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        preview.verticles = verticles.ToArray();

        UpdateCircuit();
    }

    private void UpdateCircuit(){
        var circuitPositions = circuitCalculator.Calculate(ref verticles, step).Select(x => (Vector3)x).ToArray();
        circuit.positionCount = circuitPositions.Length;
        circuit.SetPositions(circuitPositions);

        equalDistance.positionCount = verticles.Count;
        equalDistance.SetPositions(verticles.Select(x => (Vector3)x).ToArray());

        preview.equalDistance = verticles.ToArray();
        preview.circuit = circuitPositions.Select(x => (Vector2)x).ToArray();
    }
}
