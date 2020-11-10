using NUnit.Framework;

namespace EditorWindowTools.Test
{
    public class ToolBoxTest
    {
        [Test]
        public void Show()
        {
            int num1 = 0;
            var tool1 = new DelegateEditorTool<string>(() => "asdfasdf");
            tool1.onShow += () => num1++;

            int num2 = 0;
            var tool2 = new DelegateEditorTool<string>(() => "asdfasdf");
            tool2.onShow += () => num2++;

            var toolbox = new ToolBox()
            {
                tool1,
                tool2
            };

            toolbox.Show();

            Assert.AreEqual(1, num1);
            Assert.AreEqual(1, num2);
        }

        [Test]
        public void Hide()
        {
            int num1 = 0;
            var tool1 = new DelegateEditorTool<string>(() => "asdf");
            tool1.onHide += () => num1++;

            int num2 = 0;
            var tool2 = new DelegateEditorTool<string>(() => "asdf");
            tool2.onHide += () => num2++;

            var toolbox = new ToolBox()
            {
                tool1,
                tool2
            };

            toolbox.Show();
            toolbox.Hide();

            Assert.AreEqual(1, num1);
            Assert.AreEqual(1, num2);
        }

        [Test]
        public void Draw()
        {
            int num1 = 0;
            var tool1 = new DelegateEditorTool<string>(() => "asdf");
            tool1.onDraw += () => num1++;

            int num2 = 0;
            var tool2 = new DelegateEditorTool<string>(() => "asdf");
            tool2.onDraw += () => num2++;

            var toolbox = new ToolBox()
            {
                tool1,
                tool2
            };

            toolbox.Show();
            toolbox.Draw();

            Assert.AreEqual(1, num1);
            Assert.AreEqual(1, num2);
        }
    }
}