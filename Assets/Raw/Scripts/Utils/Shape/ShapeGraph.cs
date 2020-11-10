using System.Collections.Generic;
using UnityEngine;

namespace RuntimeArtWay
{
    public class ShapeGraph
    {
        private ShapeGraphVertex root;

        public ShapeGraph(ShapeGraphVertex root)
        {
            this.root = root;
        }
        
        
    }

    public class ShapeGraphVertex
    {
        private readonly List<ShapeGraphEdge> edges;

        public Vector2 Center { get; }
        public float Radius { get; }

        public ShapeGraphVertex(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;

            edges = new List<ShapeGraphEdge>();
        }

        public bool Contains(Vector2 position)
        {
            return (position - Center).magnitude < Radius;
        }

        public void Attach(List<Vector2> dots)
        {
            edges.Add(new ShapeGraphEdge(dots));
        }
    }

    public class ShapeGraphEdge
    {
        private readonly List<Vector2> dots;

        public ShapeGraphEdge(List<Vector2> dots)
        {
            this.dots = dots;
        }
    }
}