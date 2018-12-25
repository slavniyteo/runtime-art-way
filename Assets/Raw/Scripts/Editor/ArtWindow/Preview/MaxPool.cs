using System;
using System.Collections.Generic;
using System.Dynamic;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityScript.Scripting.Pipeline;

namespace RuntimeArtWay
{
    public interface IPool
    {
        Texture2D Convolute(List<Vector2> positions);
        void DrawConvolution(Rect rect, Texture2D texture);
        void DrawCachedConvolution(Rect rect, List<Vector2> positions);
    }

    public class MaxPool : IPool
    {
        private readonly float stride;
        private readonly Func<Material> getMaterial;

        private Texture2D cached;

        public MaxPool(Func<Material> getMaterial, float stride)
        {
            this.stride = stride;
            this.getMaterial = getMaterial;
        }

        public void DrawCachedConvolution(Rect rect, List<Vector2> positions)
        {
            if (!cached)
            {
                cached = Convolute(positions);
            }

            DrawConvolution(rect, cached);
        }

        public void DrawConvolution(Rect rect, Texture2D texture)
        {
            EditorGUI.DrawPreviewTexture(rect, texture, getMaterial(), ScaleMode.ScaleToFit);
        }

        public Texture2D Convolute(List<Vector2> positions)
        {
            var grid = new Grid(stride, positions);
            return grid.ToTexture();
        }
    }

    public class Grid
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Dictionary<int, bool> dict;

        public Grid(float stride, List<Vector2> positions)
        {
            Rect bounds = FindBounds(positions);
            
            Width = (int) Math.Ceiling(bounds.width / stride);
            Height = (int) Math.Ceiling(bounds.height / stride);
            
            dict = new Dictionary<int, bool>();
            Put(bounds, positions);
        }

        private void Put(Rect bounds, List<Vector2> positions)
        {
            foreach (var pos in positions)
            {
                int index = GetIndex(bounds, pos);
                dict[index] = true;
            }
        }

        private static Rect FindBounds(IEnumerable<Vector2> positions)
        {
            Rect result = Rect.MinMaxRect(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
            foreach (Vector2 p in positions)
            {
                result.xMin = Math.Min(result.xMin, p.x);
                result.yMin = Math.Min(result.yMin, p.y);
                result.xMax = Math.Max(result.xMax, p.x);
                result.yMax = Math.Max(result.yMax, p.y);
            }

            var max = Math.Max(result.size.x, result.size.y);
            result.size = new Vector2(max + 1, max + 1);

            return result;
        }

        private int GetIndex(Rect bounds, Vector2 pos)
        {
            float x_step = Math.Abs(bounds.xMax - bounds.xMin) / Width;
            int real_x = (int) Math.Floor((pos.x - bounds.xMin) / x_step);

            float y_step = Math.Abs(bounds.yMax - bounds.yMin) / Height;
            int real_y = (int) Math.Floor((pos.y - bounds.yMin) / y_step);

            return real_y * Height + real_x;
        }

        public Texture2D ToTexture()
        {
            Assert.IsTrue(dict.Count > 0);

            var result = new Texture2D(Width, Height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = result.GetPixels();

            Color32 white = new Color32(255, 255, 255, 255);
            Color32 transparent = new Color32(0, 0, 0, 0);
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = dict.ContainsKey(i) ? white : transparent;
            }

            result.SetPixels(pixels);
            result.Apply();

            return result;
        }
    }
}