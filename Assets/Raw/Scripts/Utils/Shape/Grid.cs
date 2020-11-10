using System;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Assertions;

namespace RuntimeArtWay
{
    public class Grid
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Dictionary<int, bool> dict;
        private Texture2D texture;

        private Action<Vector2> nextPoint;
        private Action<Vector2> crossPoint;

        private int lastIndex = -1;

        public Grid(float stride, List<Vector2> positions) : this(stride, positions, null, null)
        {
        }

        public Grid(float stride, List<Vector2> positions, Action<Vector2> nextPoint, Action<Vector2> crossPoint)
        {
            Rect bounds = FindBounds(positions);

            Width = (int) Math.Ceiling(bounds.width / stride);
            Height = (int) Math.Ceiling(bounds.height / stride);
            
            this.nextPoint = nextPoint;
            this.crossPoint = crossPoint;

            dict = new Dictionary<int, bool>();
            Put(bounds, positions);
        }

        private void Put(Rect bounds, List<Vector2> positions)
        {
            foreach (var pos in positions)
            {
                nextPoint?.Invoke(pos);
                int index = GetIndex(bounds, pos);

                if (index != lastIndex
                    && dict.ContainsKey(index))
                {
                    crossPoint?.Invoke(pos);
                }

                dict[index] = true;
                lastIndex = index;
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

        public Texture2D Texture
        {
            get
            {
                if (dict.Count == 0) return null;
                if (texture) return texture;

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

                texture = result;

                return result;
            }
        }
    }
}