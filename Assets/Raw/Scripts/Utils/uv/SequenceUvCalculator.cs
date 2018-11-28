using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeArtWay
{
    public class SequenceUvCalculator : IUvCalculator
    {
        public Vector2[] Calculate(TriangleNet.Mesh mesh)
        {
            var line = mesh.Vertices;

            float step = 4f / (line.Count() + 1);
            List<Vector2> circuit = new List<Vector2>()
            {
                Vector2.zero,
                Vector2.right,
                Vector2.one,
                Vector2.up
            };
            circuit = EqualDistanceUtil.Prepare(circuit, step);

            var diff = circuit.Count() - line.Count();
            return circuit.Skip(diff).ToArray();
        }
    }
}