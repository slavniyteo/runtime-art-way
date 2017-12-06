using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;

namespace EditorWindowTools.Test {
public class ToolBoxTest {

	[Test]
	public void Show() {
		int num1 = 0;
		var tool1 = new DelegateEditorTool<string>();
		tool1.onShow += () => num1++;

		int num2 = 0;
		var tool2 = new DelegateEditorTool<string>();
		tool2.onShow += () => num2++;

		var toolbox = new ToolBox<string>(){
			tool1,
			tool2
		};

		var target = "I am target";
		toolbox.Show(target);

		Assert.AreEqual(1, num1);
		Assert.AreEqual(1, num2);
	}

	[Test]
	public void Hide() {
		int num1 = 0;
		var tool1 = new DelegateEditorTool<string>();
		tool1.onHide += () => num1++;

		int num2 = 0;
		var tool2 = new DelegateEditorTool<string>();
		tool2.onHide += () => num2++;

		var toolbox = new ToolBox<string>(){
			tool1,
			tool2
		};

		var target = "I am target";
		toolbox.Show(target);
		toolbox.Hide();

		Assert.AreEqual(1, num1);
		Assert.AreEqual(1, num2);
	}

	[Test]
	public void Draw() {
		int num1 = 0;
		var tool1 = new DelegateEditorTool<string>();
		tool1.onDraw += () => num1++;

		int num2 = 0;
		var tool2 = new DelegateEditorTool<string>();
		tool2.onDraw += () => num2++;

		var toolbox = new ToolBox<string>(){
			tool1,
			tool2
		};

		var target = "I am target";
		toolbox.Show(target);
		toolbox.Draw();

		Assert.AreEqual(1, num1);
		Assert.AreEqual(1, num2);
	}
}
}