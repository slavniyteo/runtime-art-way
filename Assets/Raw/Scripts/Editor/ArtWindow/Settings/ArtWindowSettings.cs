using UnityEngine;

namespace RuntimeArtWay
{
    public interface IArtWindowSettings
    {
        Material PreviewMaterial { get; }
        string StorePath { get; }
        float CircuitRelativeStep { get; }
        Texture2D Texture { get; }
    }

    public class ArtWindowSettings : ScriptableObject, IArtWindowSettings
    {
        [SerializeField] private Material previewMaterial;

        public Material PreviewMaterial
        {
            get => previewMaterial;
            set => previewMaterial = value;
        }

        [SerializeField] private string storePath;

        public string StorePath
        {
            get
            {
                while (storePath.StartsWith("/"))
                {
                    storePath = storePath.Substring(1);
                }

                while (storePath.EndsWith("/"))
                {
                    storePath = storePath.Substring(0, storePath.Length - 1);
                }

                return storePath;
            }
            set => storePath = value;
        }

        [SerializeField] [Range(0.01f, 2f)] private float circuitRelativeStepMultiplier;

        public float CircuitRelativeStep
        {
            get => circuitRelativeStepMultiplier;
            set => circuitRelativeStepMultiplier = value;
        }

        public Texture2D texture;
        public Texture2D Texture => texture;
    }
}