using Spectre.Console.Rendering;

public interface IExplorerUi : IDisposable
{
    int LeftRatio { get; }
    int RightRatio { get; }

    event Action<ConsoleKeyInfo> OnKeyPressed;

    void MarkAsDirty();
    Task Render();
    Task StopAsync();
    void UpdateLeft(IRenderable content, string title);
    void UpdateRight(IRenderable renderable, string title);
    void UpdateRightRatio(int ratio);
    void UpdateStatus(IRenderable renderable);
}