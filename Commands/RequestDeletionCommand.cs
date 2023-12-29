namespace console_explorer.Commands;

public record RequestDeletionCommand(IExplorer explorer) : ICommand
{
    public string Name => "Delete current file or directory";

    public Task ExecuteAsync()
    {
        explorer.RequestDeleteCurrent();
        return Task.CompletedTask;
    }
}
