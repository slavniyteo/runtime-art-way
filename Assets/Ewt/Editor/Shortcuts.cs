using System;
using UnityEngine;

namespace EditorWindowTools {

    public delegate void ShortcutHandler();

    public class Shortcuts {

        public void RegisterShortcut(KeyCode shortcut, ShortcutHandler handler){
            throw new NotImplementedException();
        }

        public void Check(){
            throw new NotImplementedException();
        }

    }
}