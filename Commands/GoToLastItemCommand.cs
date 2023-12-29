namespace console_explorer.Commands;

public class GoToLastItemCommand : IUndoableCommand
{
    private readonly IExplorer explorer;
    private int oldIndex;

    public string Name => "Go to last item";

    public GoToLastItemCommand(IExplorer explorer)
    {
        this.explorer = explorer;
    }

    public Task ExecuteAsync()
    {
        oldIndex = explorer.FileIndex;
        explorer.FileIndex = explorer.ItemCount - 1;
        return Task.CompletedTask;
    }

    public Task UndoAsync()
    {
        explorer.FileIndex = oldIndex;
        return Task.CompletedTask;
    }
}
