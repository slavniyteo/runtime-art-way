namespace EditorWindowTools
{
    public delegate void BecomeDirtyHandler();

    public interface IEditorTool
    {
        void Show();

        void Hide();

        void Draw();

        event BecomeDirtyHandler onDirty;
    }
}