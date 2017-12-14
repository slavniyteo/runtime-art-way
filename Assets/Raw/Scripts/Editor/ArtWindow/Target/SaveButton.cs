using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EditorWindowTools;
using RectEx;

namespace RuntimeArtWay {
    public class SaveButton {

        private GUIStyle nameTemporary;
        private GUIStyle namePersistent;
        private GUIStyle nameConflict;

        private event Action<Sample, Sample> updateAsset;

        public SaveButton(Action<Sample, Sample> updateAsset){
            if (updateAsset == null) throw new ArgumentNullException("UpdateAsset event is null");

            this.updateAsset = updateAsset;
        }

        public void PrepareGUI(){
            nameTemporary = new GUIStyle(GUI.skin.box);
            nameTemporary.normal.background = TextureGenerator.GenerateBox(10, 10, Color.gray);

            namePersistent = new GUIStyle(GUI.skin.box);
            namePersistent.normal.background = TextureGenerator.GenerateBox(10, 10, Color.green);

            nameConflict = new GUIStyle(GUI.skin.box);
            nameConflict.normal.background = TextureGenerator.GenerateBox(10, 10, Color.cyan);
        }

        public void Draw(Rect rect, Sample target){
            bool isPersistent = EditorUtility.IsPersistent(target);
            var path = isPersistent 
                            ? AssetDatabase.GetAssetPath(target) 
                            : string.Format("Assets/{0}.asset", target.name);
            var objectAtPath = AssetDatabase.LoadAssetAtPath(path, typeof(Sample));

            if (objectAtPath != null) {
                if (objectAtPath == target){
                    GUI.Box(rect, "S", namePersistent);
                }
                else {
                    GUI.Box(rect, "S", nameConflict);
                }
            }
            else {
                if (isPersistent){
                    GUI.Box(rect, "S", namePersistent);
                }
                else if (GUI.Button(rect, "S", nameTemporary)){
                    AssetDatabase.CreateAsset(target, path);
                    AssetDatabase.ImportAsset(path);
                    var newTarget = AssetDatabase.LoadAssetAtPath(path, typeof(Sample)) as Sample;
                    updateAsset(target, newTarget);
                }
            }
        }
    }
}