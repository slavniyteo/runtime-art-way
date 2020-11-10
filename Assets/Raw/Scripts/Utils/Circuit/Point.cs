using UnityEngine;

namespace RuntimeArtWay.Circuit
{
    public class Point

{
        public Vector2 Position { get; set; }
        public bool Enabled { get; set; }

        public Point(Vector2 position)
        {
            Position = position;
            Enabled = true;
        }

        public Point(Vector2 position, bool enabled)
        {
            Position = position;
            Enabled = enabled;
        }

        public override string ToString()
        {
            return $"{Enabled}; {Position}";
        }
    }
}