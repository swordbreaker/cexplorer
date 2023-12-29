namespace console_explorer.Commands;

public class GoBackADirectoryCommand : IUndoableCommand
{
    private readonly IExplorer explorer;
    private DirectoryInfo? oldDirectory;

    public string Name => "Go back a directory";

    public GoBackADirectoryCommand(IExplorer explorer)
    {
        this.explorer = explorer;
    }

    public Task ExecuteAsync()
    {
        oldDirectory = explorer.WorkingDirectory;
        explorer.GoBackADirectory();
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
