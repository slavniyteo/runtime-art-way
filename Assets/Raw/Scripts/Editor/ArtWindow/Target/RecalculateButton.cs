using EditorWindowTools;
using UnityEngine;

namespace RuntimeArtWay
{
    public class RecalculateButton
    {

        private IArtWindowSettings settings;
        
        public RecalculateButton(IArtWindowSettings settings)
        {
            this.settings = settings;
        }

        public void Draw(Rect rect, Sample target)
        {
            if (GUI.Button(rect, "R"))
            {
                SampleBuilder.Rebuild(target, settings.CircuitStep);
            }
        }
    }
}