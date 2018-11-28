using UnityEngine;

namespace EditorWindowTools
{
    public delegate void DrawOverviewDelegate();

    public class Overview
    {
        public Overview(string message)
        {
        }

        public Overview(DrawOverviewDelegate draw)
        {
        }

        public void Draw(Rect rect)
        {
        }

        public void DrawLayout()
        {
        }
    }
}