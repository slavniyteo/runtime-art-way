using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;

namespace RuntimeArtWay {
    public class MeshGenerator {

        private static TriangleNet.Behavior Behavior = new TriangleNet.Behavior{
                ConformingDelaunay = true,
                NoBisect = 0
            };

        public static TriangleNet.Mesh Generate(List<Vector2> circuit) {
            var mesh = new TriangleNet.Mesh(Behavior);

            var geometry = GetGeometryFor(circuit);
            mesh.Triangulate(geometry);

            return mesh;
        }

        private static InputGeometry GetGeometryFor(List<Vector2> circuit){
            var result = new TriangleNet.Geometry.InputGeometry(circuit.Count);

            circuit.ForEach(v => result.AddPoint(v.x, v.y));
            for (int i = 0; i < circuit.Count - 1; i++){
                result.AddSegment(i, i + 1, 1);
            }

            result.AddSegment(circuit.Count - 1, 0, 1);

            return result;
        }
    }
}