namespace console_explorer.Commands.Navigation;

public class ChangeItemSelectionCommand : ICommand
{
    private readonly IExplorer explorer;
    private readonly int indexChange;
    public string Name => indexChange < 0
        ? "Select previous item"
        : "Select next item";

    public ChangeItemSelectionCommand(IExplorer explorer, int indexChange)
    {
        this.explorer = explorer;
        this.indexChange = indexChange;
    }

    public Task ExecuteAsync()
    {
        explorer.FileIndex += indexChange;
        return Task.CompletedTask;
    }
}