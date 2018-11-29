using EditorWindowTools;
using UnityEngine;

namespace RuntimeArtWay
{
    public class RecalculateButton 
    {
        public void Draw(Rect rect, Sample target)
        {
            if (GUI.Button(rect, "R"))
            {
                SampleBuilder.Rebuild(target, 5);
            }
        }
    }
}