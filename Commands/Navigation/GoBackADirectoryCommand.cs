namespace console_explorer.Commands.Navigation;

public class GoBackADirectoryCommand : ICommand
{
    private readonly IExplorer explorer;

    public string Name => "Go back a directory";

    public GoBackADirectoryCommand(IExplorer explorer)
    {
        this.explorer = explorer;
    }

    public Task ExecuteAsync()
    {
        explorer.GoBackADirectory();
        return Task.CompletedTask;
    }
}
