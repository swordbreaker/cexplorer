namespace console_explorer.Commands;

public class ChangeItemSelectionCommand : IUndoableCommand
{
    private readonly IExplorer explorer;
    private readonly int indexChange;
    private int oldIndex;

    public string Name => indexChange < 0
        ? "Select previous item"
        : "Select next item";

    public ChangeItemSelectionCommand(IExplorer explorer, int indexChange)
    {
        this.explorer = explorer;
        this.indexChange = indexChange;
    }


    public Task ExecuteAsync()
    {
        oldIndex = explorer.FileIndex;
        explorer.FileIndex += indexChange;
        return Task.CompletedTask;
    }

    public Task UndoAsync()
    {
        explorer.FileIndex = oldIndex;
        return Task.CompletedTask;
    }
}