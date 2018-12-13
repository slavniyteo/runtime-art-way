using System;
using System.Collections.Generic;
using UnityEngine;

namespace EditorWindowTools
{
    public class AbstractEditorTool<T> : IEditorTool
        where T : class
    {
        protected readonly Func<T> getNewTarget;
        internal T target { get; private set; }
        internal bool Active { get; private set; }

        private bool guiPrepared = false;

        public AbstractEditorTool(Func<T> getNewTarget)
        {
            this.getNewTarget = getNewTarget ?? throw new ArgumentException("Target getter must not be null");
        }

        public void Show()
        {
            if (Active) Hide();

            target = getNewTarget();
            if (target == null) throw new ArgumentException("Target must be not null");

            Active = true;

            OnShow();
        }

        public void Hide()
        {
            bool needCallOnHide = Active;

            target = null;
            Active = false;

            guiPrepared = false;

            if (needCallOnHide)
            {
                OnHide();
            }
        }

        public void Draw()
        {
            if (target == null) throw new ArgumentException("Target must be not null");
            if (!Active) throw new InvalidOperationException("Can not draw while is not active");

            if (!guiPrepared)
            {
                PrepareGUI();
                guiPrepared = true;
            }

            OnDraw();
        }

        public void DrawOnce()
        {
            Show();
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

        protected void SetUndo(Action undo)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}