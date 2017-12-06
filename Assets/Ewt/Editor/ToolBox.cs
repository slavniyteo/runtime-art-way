using System.Collections;
using System.Collections.Generic;

namespace EditorWindowTools {

    public class ToolBox<T> : IEditorTool<T>, IEnumerable<IEditorTool<T>> {

        private List<IEditorTool<T>> tools = new List<IEditorTool<T>>();

        public void Show(T target) {
            foreach (var tool in tools) {
                tool.Show(target);
            }
        }

        public void Hide() {
            foreach (var tool in tools) {
                tool.Hide();
            }
        }

        public void Draw() {
            foreach (var tool in tools) {
                tool.Draw();
            }
        }

        public event BecomeDirtyHandler onDirty {
            add {
                foreach (var tool in tools) {
                    tool.onDirty += value;
                }
            }
            remove {
                foreach (var tool in tools) {
                    tool.onDirty -= value;
                }
            }
        }

        #region IEnumerable

        public void Add(IEditorTool<T> tool){
            if (tool == null) throw new System.ArgumentNullException();
            tools.Add(tool);
        }

        public IEnumerator<IEditorTool<T>> GetEnumerator() {
            return tools.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}