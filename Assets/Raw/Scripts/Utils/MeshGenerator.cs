using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeArtWay
{
    public class MeshGenerator
    {
        private IUvCalculator uvCalculator;

        public MeshGenerator(UvAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case UvAlgorithm.Sequence:
                {
                    uvCalculator = new SequenceUvCalculator();
                    break;
                }
                case UvAlgorithm.Mask:
                {
                    uvCalculator = new MaskUvCalculator();
                    break;
                }
                default:
                {
                    throw new NotImplementedException();
                }
            }
        }

        public Mesh Generate(IEnumerable<Vector2> line)
        {
            var mesh = ThirdPartyMeshGenerator.Generate(line);

            var result = new Mesh();
            result.vertices = CalculateVertices(mesh);
            result.triangles = CalculateTriangles(mesh);
            result.uv = uvCalculator.Calculate(mesh);

            return result;
        }

        public static Vector3[] CalculateVertices(TriangleNet.Mesh mesh)
        {
            return mesh.Vertices.Select(x => new Vector3((float) x.X, (float) x.Y)).ToArray();
        }

        private static int[] CalculateTriangles(TriangleNet.Mesh mesh)
        {
            return mesh.Triangles.SelectMany(t => new int[] {t.P0, t.P1, t.P2}).ToArray();
        }
    }
}