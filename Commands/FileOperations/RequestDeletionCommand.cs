namespace console_explorer.Commands.FileOperations;

public record RequestDeletionCommand(IExplorer explorer) : ICommand
{
    public string Name => "Delete current file or directory";

    public Task ExecuteAsync()
    {
        explorer.RequestDeleteCurrent();
        return Task.CompletedTask;
    }
}
