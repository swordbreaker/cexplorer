namespace console_explorer.Commands;

public record StartRenameCommand(IExplorer explorer) : ICommand
{
    public string Name => "Rename the directory of file";

    public Task ExecuteAsync()
    {
        explorer.StartRenameCurrent();
        return Task.CompletedTask;
    }
}
