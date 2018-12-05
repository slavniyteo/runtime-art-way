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

        private AbstractEditorTool<string> create(string target = "")
        {
            return new AbstractEditorTool<string>(() => target);
        } 
        
        private AbstractEditorTool<string> create(Func<string> getTarget)
        {
            return new AbstractEditorTool<string>(getTarget);
        } 
        
        [Test]
        public void IsActiveBeforeShow()
        {
            var tool = create();
            Assert.IsFalse(tool.Active);
        }

        [Test]
        public void Show()
        {
            var target = "I am target";
            var tool = create(target);
            tool.Show();

            Assert.AreEqual(target, tool.target);
            Assert.IsTrue(tool.Active);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShowNullTarget()
        {
            var tool = create(() => null);
            tool.Show();
        }

        [Test]
        public void ShowTwice()
        {
            var target = "I am target";
            var tool = create(() => target);
            tool.Show();
            target = "I am another target";
            tool.Show();

            Assert.AreEqual(target, tool.target);
        }

        [Test]
        public void Hide()
        {
            var tool = create();
            tool.Show();
            tool.Hide();

            Assert.AreEqual(null, tool.target);
            Assert.IsFalse(tool.Active);
        }

        [Test]
        public void HideBeforeShow()
        {
            var tool = new DelegateEditorTool<string>(() => "");
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
            var tool = new DelegateEditorTool<string>(() => "");
            tool.Show();

            int num = 0;
            tool.onHide += () => num++;
            tool.Hide();
            tool.Hide();

            Assert.AreEqual(1, num, "OnHide was called twice");
        }

        [Test]
        public void Draw()
        {
            var tool = create();
            tool.Show();
            tool.Draw();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DrawInNotActive()
        {
            var tool = create();
            tool.Draw();
        }

        [Test]
        public void OnShow()
        {
            var tool = new DelegateEditorTool<string>(() => "");
            var num = 0;
            tool.onShow += () => num++;

            tool.Show();

            Assert.AreEqual(1, num);
        }

        [Test]
        public void OnHide()
        {
            var tool = new DelegateEditorTool<string>(() => "");
            var num = 0;
            tool.onHide += () => num++;

            tool.Show();
            tool.Hide();

            Assert.AreEqual(1, num);
        }

        [Test]
        public void OnDraw()
        {
            var tool = new DelegateEditorTool<string>(() => "");
            var num = 0;
            tool.onDraw += () => num++;

            tool.Show();
            tool.Draw();

            Assert.AreEqual(1, num);
        }
    }
}