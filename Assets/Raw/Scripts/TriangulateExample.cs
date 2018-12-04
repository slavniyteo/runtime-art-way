using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RuntimeArtWay.Circuit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeArtWay
{
    [ExecuteInEditMode]
    public class TriangulateExample : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public LineRenderer line;
        public LineRenderer circuit;
        public LineRenderer equalDistance;

        public float step = 10;

        private List<Vector2> verticles;

        void Start()
        {
            verticles = new List<Vector2>();
        }

        void Update()
        {
            if (line.positionCount != verticles.Count)
            {
                line.positionCount = verticles.Count;
                line.SetPositions(verticles.Select(x => (Vector3) x).ToArray());
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            verticles.Clear();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            verticles.Add(eventData.position);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            UpdateCircuit();
        }

        private void UpdateCircuit()
        {
            var sample = SampleBuilder.CreateSample(verticles, step);

            var equalDistanceCloud = sample.equalDistance;
            equalDistance.positionCount = equalDistanceCloud.Count;
            equalDistance.SetPositions(equalDistanceCloud.Select(x => (Vector3) x).ToArray());

            var circuitPositions = sample.circuit;
            circuit.positionCount = circuitPositions.Count;
            circuit.SetPositions(circuitPositions.Select(x => (Vector3) x).ToArray());
        }
    }
}