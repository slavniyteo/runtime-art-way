using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeArtWay
{
    public class TriangulateExample : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private static readonly MeshGenerator meshGenerator = new MeshGenerator(UvAlgorithm.Mask);

        [Header("Settings")] public BagOfRequests bagOfRequests;
        [Range(0.1f, 2)] public float stepMultiplier = 0.3f;
        [Range(1, 100)] public float minStep = 1;

        [Header("References")] public LineRenderer handMade;
        public LineRenderer circuit;
        public LineRenderer equalDistance;

        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;

        public ParticleSystem burningHandMade;
        [Range(0, 1)] public float burningStep = 0;
        private bool burned;

        private List<Vector2> positions;
        private bool building = false;

        private void Start()
        {
            positions = new List<Vector2>();
        }

        private void Update()
        {
            if (building) return;
            if (handMade.positionCount == positions.Count) return;

            handMade.positionCount = positions.Count;
            handMade.SetPositions(positions.Select(x => (Vector3) x).ToArray());
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (building) return;
            positions.Clear();

            equalDistance.positionCount = 0;
            equalDistance.SetPositions(new Vector3[0]);

            circuit.positionCount = 0;
            circuit.SetPositions(new Vector3[0]);

            meshFilter.mesh = null;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (building) return;
            if (positions.Count > 0
                && (positions[positions.Count - 1] - eventData.position).magnitude < minStep)
            {
                return;
            }

            positions.Add(eventData.position);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (building) return;
            building = true;
            StartCoroutine(BurnHandMade());
            StartCoroutine(UpdateCircuit());
        }

        private IEnumerator BurnHandMade()
        {
            var positions = new Vector3[handMade.positionCount];
            handMade.GetPositions(positions);

            burningHandMade.transform.localPosition = positions[0];
            burningHandMade.Play();

            var wait = new WaitForSeconds(burningStep);
            for (int i = 0; i < positions.Length; i++)
            {
                burningHandMade.transform.localPosition = positions[i];

                var newPositions = positions.Skip(i).ToArray();
                handMade.SetPositions(newPositions);
                handMade.positionCount = newPositions.Length;

                yield return wait;
            }

            burningHandMade.Stop();
            burned = true;
        }

        private IEnumerator UpdateCircuit()
        {
            var sample = SampleBuilder.CreateSample(positions, stepMultiplier);
            yield return null;

            while (!burned)
            {
                yield return null;
            }

            SetCircuit(sample);
            yield return null;

            SetMesh(sample.Circuit);
            SetTexture();

            burned = false;
            building = false;
        }

        private void SetCircuit(ISample sample)
        {
            if (equalDistance.enabled)
            {
                var equalDistanceCloud = sample.EqualDistance;
                equalDistance.positionCount = equalDistanceCloud.Count;
                equalDistance.SetPositions(equalDistanceCloud.Select(x => (Vector3) x).ToArray());
            }

            var circuitPositions = sample.Circuit;
            circuit.positionCount = circuitPositions.Count;
            circuit.SetPositions(circuitPositions.Select(x => (Vector3) x).ToArray());
        }

        private void SetMesh(List<Vector2> vertices)
        {
            var mesh = meshGenerator.Generate(vertices);
            meshFilter.sharedMesh = mesh;
        }

        private void SetTexture()
        {
            var texture = bagOfRequests.RandomRequest.Texture;
            meshRenderer.sharedMaterials[0].SetTexture("_MainTex", texture);
        }
    }
}