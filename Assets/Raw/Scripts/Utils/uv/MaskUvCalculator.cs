using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeArtWay {
    public class MaskUvCalculator : IUvCalculator {

        public Vector2[] Calculate(TriangleNet.Mesh mesh) {
            var line = mesh.Vertices.Select(v => new Vector2((float)v.X, (float)v.Y));

			Vector2 min = line.Aggregate((v, res) => new Vector2(Mathf.Min(v.x, res.x), Mathf.Min(v.y, res.y)));
			Vector2 max = line.Aggregate((v, res) => new Vector2(Mathf.Max(v.x, res.x), Mathf.Max(v.y, res.y)));
			max = max - min;

            line = line.Select(v => v - min);

            return line.Select(v => {
                return new Vector2(
                    x: v.x / max.x,
                    y: v.y / max.y
                );
            }).ToArray();

        }
        
    }
}