using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;

namespace RuntimeArtWay
{
    public class ThirdPartyMeshGenerator
    {
        private static TriangleNet.Behavior Behavior = new TriangleNet.Behavior
        {
            ConformingDelaunay = true,
            NoBisect = 0
        };

        public static TriangleNet.Mesh Generate(IEnumerable<Vector2> circuit)
        {
            var mesh = new TriangleNet.Mesh(Behavior);

            var geometry = GetGeometryFor(circuit);
            mesh.Triangulate(geometry);

            return mesh;
        }

        private static InputGeometry GetGeometryFor(IEnumerable<Vector2> circuit)
        {
            var result = new TriangleNet.Geometry.InputGeometry(circuit.Count());

            foreach (var v in circuit)
            {
                result.AddPoint(v.x, v.y);
            }

            for (int i = 0; i < result.Count - 1; i++)
            {
                result.AddSegment(i, i + 1, 1);
            }

            result.AddSegment(result.Count - 1, 0, 1);

            return result;
        }
    }
}