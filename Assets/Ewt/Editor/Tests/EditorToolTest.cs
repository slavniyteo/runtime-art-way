using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;

namespace EditorWindowTools.Test
{
    public class EditorToolTest
    {
        [Test]
        public void IsActiveBeforeShow()
        {
            var tool = new AbstractEditorTool<string>();
            Assert.IsFalse(tool.Active);
        }

        [Test]
        public void Show()
        {
            var tool = new AbstractEditorTool<string>();
            var target = "I am target";
            tool.Show(target);

            Assert.AreEqual(target, tool.target);
            Assert.IsTrue(tool.Active);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShowNullTarget()
        {
            var tool = new AbstractEditorTool<string>();
            tool.Show(null);
        }

        [Test]
        public void ShowTwice()
        {
            var tool = new AbstractEditorTool<string>();
            var target = "I am target";
            tool.Show(target);
            var target2 = "I am another target";
            tool.Show(target2);

            Assert.AreEqual(target2, tool.target);
        }

        [Test]
        public void Hide()
        {
            var tool = new AbstractEditorTool<string>();
            var target = "I am target";
            tool.Show(target);
            tool.Hide();

            Assert.AreEqual(null, tool.target);
            Assert.IsFalse(tool.Active);
        }

        [Test]
        public void HideBeforeShow()
        {
            var tool = new DelegateEditorTool<string>();
            int num = 0;
            tool.onHide += () => num++;
            tool.Hide();

            Assert.AreEqual(null, tool.target);
            Assert.IsFalse(tool.Active);
            Assert.AreEqual(0, num, "OnHide was called");
        }

        [Test]
        public void HideTwice()
        {
            var tool = new DelegateEditorTool<string>();
            var target = "I am target";
            tool.Show(target);

            int num = 0;
            tool.onHide += () => num++;
            tool.Hide();
            tool.Hide();

            Assert.AreEqual(1, num, "OnHide was called twice");
        }

        [Test]
        public void Draw()
        {
            var tool = new AbstractEditorTool<string>();
            var target = "I am target";
            tool.Show(target);
            tool.Draw();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DrawInNotActive()
        {
            var tool = new AbstractEditorTool<string>();
            tool.Draw();
        }

        [Test]
        public void OnShow()
        {
            var tool = new DelegateEditorTool<string>();
            var num = 0;
            tool.onShow += () => num++;

            tool.Show("I am target");

            Assert.AreEqual(1, num);
        }

        [Test]
        public void OnHide()
        {
            var tool = new DelegateEditorTool<string>();
            var num = 0;
            tool.onHide += () => num++;

            tool.Show("");
            tool.Hide();

            Assert.AreEqual(1, num);
        }

        [Test]
        public void OnDraw()
        {
            var tool = new DelegateEditorTool<string>();
            var num = 0;
            tool.onDraw += () => num++;

            tool.Show("");
            tool.Draw();

            Assert.AreEqual(1, num);
        }
    }
}