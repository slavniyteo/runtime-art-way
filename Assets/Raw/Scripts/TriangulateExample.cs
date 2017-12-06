using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeArtWay {
[ExecuteInEditMode]
public class TriangulateExample : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

    public Sample preview;
    public LineRenderer line;
    public LineRenderer circuit;
    public LineRenderer equalDistance;
    public Text number;

    public float step = 10;

    private CircuitCalculator circuitCalculator = new CircuitCalculator();

    private List<Vector2> verticles;

    void Start () {
        verticles = new List<Vector2>();
        ClearNumbers();
    }

    [ContextMenu("Draw Lines")]
    public void DrawLines(){
        verticles = preview.verticles.ToList();

        UpdateCircuit();
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


    private List<GameObject> instances = new List<GameObject>();

    public Transform parentPattern;
    private Transform parent;
    private void UpdateCircuit(){
        var circuitPositions = circuitCalculator.Calculate(ref verticles, step).Select(x => (Vector3)x).ToArray();
        circuit.positionCount = circuitPositions.Length;
        circuit.SetPositions(circuitPositions);

        equalDistance.positionCount = verticles.Count;
        equalDistance.SetPositions(verticles.Select(x => (Vector3)x).ToArray());

        preview.equalDistance = verticles.ToArray();
        preview.circuit = circuitPositions.Select(x => (Vector2)x).ToArray();

        for (int i = 0; i < circuitPositions.Length; i++){
            var verticle = circuitPositions[i];
            var instance = Instantiate(number.gameObject, verticle, Quaternion.identity);
            instance.SetActive(true);
            instance.transform.SetParent(parent, false);
            instances.Add(instance);
            int j = i;
            instance.GetComponent<Text>().text = "" + j;
        }
    }

    [ContextMenu("Clear Instances")]
    public void ClearNumbers(){
        if (parent){
            DestroyImmediate(parent.gameObject);
        }
        parent = Instantiate(parentPattern.gameObject, Vector3.zero, Quaternion.identity).transform;
        parent.SetParent(parentPattern.parent, false);

        instances.Clear();
    }
}
}