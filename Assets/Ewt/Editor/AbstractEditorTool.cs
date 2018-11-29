using System;
using System.Collections.Generic;
using UnityEngine;

namespace EditorWindowTools
{
    public class AbstractEditorTool<T> : IEditorTool<T>
    {
        internal T target { get; private set; }
        internal bool Active { get; private set; }

        public event BecomeDirtyHandler onDirty;

        private bool guiPrepared = false;

        public void Show(T target)
        {
            if (target == null) throw new ArgumentException("Target must be not null");

            if (Active) Hide();

            this.target = target;
            Active = true;

            OnShow();
        }

        public void Hide()
        {
            bool needCallOnHide = Active;

            this.target = default(T);
            Active = false;

            guiPrepared = false;

            if (needCallOnHide)
            {
                OnHide();
            }
        }

        public void Draw()
        {
            if (!Active) throw new InvalidOperationException("Can not draw while is not active");

            if (!guiPrepared)
            {
                PrepareGUI();
                guiPrepared = true;
            }

            OnDraw();
        }

        public void DrawOnce(T target)
        {
            Show(target);
            Draw();
            Hide();
        }

        #region Virtual

        protected virtual void PrepareGUI()
        {
        }

        protected virtual void OnDraw()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual Overview Overview { get; }

        protected virtual Dictionary<KeyCode, ShortcutHandler> Shortcuts { get; }

        #endregion

        #region Protected

        protected void SetDirty()
        {
            onDirty?.Invoke();
        }

        protected void SetUndo(Action undo)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}