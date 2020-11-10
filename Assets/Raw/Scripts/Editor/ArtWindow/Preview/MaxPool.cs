using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RuntimeArtWay
{
    public interface IPool
    {
        void Draw(Rect rect);
    }

    public class MaxPool : IPool
    {
        private readonly Grid grid;
        private readonly Func<Material> getMaterial;

        public MaxPool(Func<Material> getMaterial, float stride, List<Vector2> positions)
        {
            this.getMaterial = getMaterial;
            grid = new Grid(stride, positions);
            
            var graph = new Graph(getMaterial, positions, stride);
        }

        public void Draw(Rect rect)
        {
            if (grid.Texture)
            {
                EditorGUI.DrawPreviewTexture(rect, grid.Texture, getMaterial(), ScaleMode.ScaleToFit);
            }
        }
    }
}