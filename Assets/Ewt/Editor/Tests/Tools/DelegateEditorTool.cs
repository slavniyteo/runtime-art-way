using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;

namespace EditorWindowTools.Test {
	public class DelegateEditorTool<T> : AbstractEditorTool<T> {
		public event Action onShow;
		protected override void OnShow(){
			if (onShow != null) {
				onShow();
			}
		}

		public event Action onHide;
		protected override void OnHide(){
			if (onHide != null) {
				onHide();
			}
		}

		public event Action onDraw;
		protected override void OnDraw(){
			if (onDraw != null) {
				onDraw();
			}
		}


	}

}