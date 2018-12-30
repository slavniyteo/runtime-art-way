using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace RuntimeArtWay
{
    public interface IGraph
    {
        void Draw(Rect rect);
    }

    public class Graph : IGraph
    {
        private readonly Func<Material> getMaterial;

        private Texture2D Texture => GraphToTexture();
        private List<Grid> parts;

        public Graph(Func<Material> getMaterial, List<Vector2> positions, float stride)
        {
            this.getMaterial = getMaterial;

            var graphBuilder = new GraphBuilder(positions, stride);
            parts = graphBuilder.Build();
        }

        public void Draw(Rect rect)
        {
            foreach (var grid in parts)
            {
                EditorGUI.DrawPreviewTexture(rect, grid.Texture, getMaterial(), ScaleMode.ScaleToFit);
            }
        }

        private Texture2D GraphToTexture(asdfg )
        {
            return null;
        }
    }

    public class GraphBuilder
    {
        private readonly List<Vector2> dots;
        private readonly float stride;

        private readonly Stack<List<Vector2>> stack = new Stack<List<Vector2>>();
        private readonly LinkedList<Vector2> current = new LinkedList<Vector2>();

        public GraphBuilder(List<Vector2> dots, float stride)
        {
            this.dots = dots;
            this.stride = stride;
        }

        public List<Grid> Build()
        {
            new Grid(stride, dots, NextPoint, CrossPoint);
            return stack.Select(x => new Grid(stride, x)).ToList();
        }

        private void NextPoint(Vector2 position)
        {
            current.AddFirst(position);
        }

        private void CrossPoint(Vector2 position)
        {
            var newLine = new List<Vector2> {position};

            var target = current.First;
            bool left = false;
            while (!left || (position - target.Value).magnitude > stride)
            {
                if (!left && (position - target.Value).magnitude > stride)
                {
                    left = true;
                }

                newLine.Add(target.Value);
                current.RemoveFirst();
                target = current.First;
            }

            newLine.Add(target.Value);

            Assert.IsTrue((newLine[0] - position).magnitude <= stride);
            Assert.IsTrue((newLine[newLine.Count - 1] - position).magnitude < stride);

            stack.Push(newLine);
            NextPoint(position);
        }
    }
}