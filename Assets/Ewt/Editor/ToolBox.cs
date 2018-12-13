using System;
using System.Collections;
using System.Collections.Generic;

namespace EditorWindowTools
{
    public class ToolBox : IEditorTool, IEnumerable<IEditorTool>
    {
        private List<IEditorTool> tools = new List<IEditorTool>();

        public void Show()
        {
            foreach (var tool in tools)
            {
                tool.Show();
            }
        }

        public void Hide()
        {
            foreach (var tool in tools)
            {
                tool.Hide();
            }
        }

        public void Draw()
        {
            foreach (var tool in tools)
            {
                tool.Draw();
            }
        }

        #region IEnumerable

        public void Add(IEditorTool tool)
        {
            if (tool == null) throw new ArgumentNullException();
            tools.Add(tool);
        }

        public IEnumerator<IEditorTool> GetEnumerator()
        {
            return tools.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}