using System;
using UnityEngine;

namespace RuntimeArtWay {

    public interface IArtWindowSettings {
        Material PreviewMaterial { get; }
        string StorePath { get; }
    }
    public class ArtWindowSettings : ScriptableObject, IArtWindowSettings {

        [SerializeField] 
        private Material previewMaterial;
        public Material PreviewMaterial { 
            get { return previewMaterial; } 
            set { previewMaterial = value; }
        }

        [SerializeField]
        private string storePath;
        public string StorePath { 
            get { 
                while (storePath.StartsWith("/")){
                    storePath = storePath.Substring(1);
                }
                while (storePath.EndsWith("/")){
                    storePath = storePath.Substring(0, storePath.Length - 1);
                }
                return storePath; 
            } 
            set { storePath = value; }
        }

    }
}