using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriangulateExample : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

    public TriangulatePreview preview;
    public LineRenderer line;
    public LineRenderer circuit;

    private CircuitCalculator circuitCalculator;

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
        verticles.Add(eventData.position);
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        verticles.Add(eventData.position);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        verticles.Add(eventData.position);
        preview.verticles = verticles.ToArray();

        UpdateCircuit();
    }

    private void UpdateCircuit(){
        var circuitPositions = circuitCalculator.Calculate(verticles, 0.1f).Select(x => (Vector3)x).ToArray();
        circuit.SetPositions(circuitPositions);
    }
}
