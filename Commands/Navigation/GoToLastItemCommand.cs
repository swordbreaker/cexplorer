namespace console_explorer.Commands.Navigation;

public class GoToLastItemCommand : ICommand
{
    private readonly IExplorer explorer;
    public string Name => "Go to last item";
    public GoToLastItemCommand(IExplorer explorer)
    {
        this.explorer = explorer;
    }
    public Task ExecuteAsync()
    {
        explorer.FileIndex = explorer.ItemCount - 1;
        return Task.CompletedTask;
    }
}
