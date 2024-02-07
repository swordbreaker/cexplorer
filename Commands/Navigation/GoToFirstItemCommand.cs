namespace console_explorer.Commands.Navigation;

public class GoToFirstItemCommand : ICommand
{
    private readonly IExplorer explorer;

    public string Name => "Go to first item";

    public GoToFirstItemCommand(IExplorer explorer)
    {
        this.explorer = explorer;
    }

    public Task ExecuteAsync()
    {
        explorer.FileIndex = 0;
        return Task.CompletedTask;
    }
}
