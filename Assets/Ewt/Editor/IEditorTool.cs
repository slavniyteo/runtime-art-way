namespace EditorWindowTools
{
    public delegate void BecomeDirtyHandler();

    public interface IEditorTool<T>
    {
        void Show(T target);

        void Hide();

        void Draw();

        event BecomeDirtyHandler onDirty;
    }
}