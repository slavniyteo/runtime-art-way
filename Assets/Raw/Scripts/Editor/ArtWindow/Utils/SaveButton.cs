using System;
using UnityEditor;
using UnityEngine;

namespace RuntimeArtWay
{
    public class SaveButton
    {
        private GUIStyle nameTemporary;
        private GUIStyle namePersistent;
        private GUIStyle nameConflict;

        private readonly Func<String> getStorePath;

        public SaveButton(Func<String> getStorePath)
        {
            this.getStorePath = getStorePath;
        }

        public void PrepareGUI()
        {
            nameTemporary = new GUIStyle(GUI.skin.box)
            {
                normal = {background = TextureGenerator.GenerateBox(10, 10, Color.gray)}
            };

            namePersistent = new GUIStyle(GUI.skin.box)
            {
                normal = {background = TextureGenerator.GenerateBox(10, 10, Color.green)}
            };

            nameConflict = new GUIStyle(GUI.skin.box)
            {
                normal = {background = TextureGenerator.GenerateBox(10, 10, Color.cyan)}
            };
        }

        public void Draw<T>(Rect rect, T target, Action<T, T> updateAsset) where T : class
        {
            if (!(target is ScriptableObject)) return;
            if (updateAsset == null) throw new ArgumentNullException(nameof(updateAsset));

            ScriptableObject sample = target as ScriptableObject;
            bool isPersistent = EditorUtility.IsPersistent(sample);
            var path = isPersistent
                ? AssetDatabase.GetAssetPath(sample)
                : $"Assets/{getStorePath()}/{sample.name}.asset";
            var objectAtPath = AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject));

            if (objectAtPath != null)
            {
                GUI.Box(rect, "S",
                    ReferenceEquals(objectAtPath, sample) ? namePersistent : nameConflict
                );
            }
            else
            {
                if (isPersistent)
                {
                    GUI.Box(rect, "S", namePersistent);
                }
                else if (GUI.Button(rect, "S", nameTemporary))
                {
                    AssetDatabase.CreateAsset(sample, path);
                    AssetDatabase.ImportAsset(path);
                    var newTarget = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                    updateAsset(target, newTarget);
                }
            }
        }
    }
}