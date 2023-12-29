namespace console_explorer.Commands;

public record QuitCommand(IExplorer Explorer) : ICommand
{
    public string Name => "Quit";

    public Task ExecuteAsync()
    {
        Explorer.Dispose();
        return Task.CompletedTask;
    }
}
