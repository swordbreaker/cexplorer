namespace console_explorer.Commands;

public class ExecuteCurrentItemCommand : IUndoableCommand
{
    private readonly IExplorer explorer;
    private DirectoryInfo? oldDirectory;

    public ExecuteCurrentItemCommand(IExplorer explorer)
    {
        this.explorer = explorer;
    }

    public string Name => "Open directory or open file with the default application.";

    public Task ExecuteAsync()
    {
        oldDirectory = explorer.WorkingDirectory;
        explorer.ExecuteSelectedItem();
        return Task.CompletedTask;
    }

    public Task UndoAsync()
    {
        if(oldDirectory != null)
        {
            explorer.EnterDirectory(oldDirectory);
        }
        return Task.CompletedTask;
    }
}
