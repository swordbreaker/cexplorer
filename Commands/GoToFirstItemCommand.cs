namespace console_explorer.Commands;

public class GoToFirstItemCommand : IUndoableCommand
{
    private readonly IExplorer explorer;
    private int oldIndex;

    public string Name => "Go to first item";

    public GoToFirstItemCommand(IExplorer explorer)
    {
        this.explorer = explorer;
    }

    public Task ExecuteAsync()
    {
        oldIndex = explorer.FileIndex;
        explorer.FileIndex = 0;
        return Task.CompletedTask;
    }

    public Task UndoAsync()
    {
        explorer.FileIndex = oldIndex;
        return Task.CompletedTask;
    }
}
