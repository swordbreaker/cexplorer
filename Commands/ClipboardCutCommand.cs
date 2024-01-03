namespace console_explorer.Commands;

public class ClipboardCutCommand : ICommand
{
    public string Name => "Cut to clipboard";
    private readonly ICliboardService clipboardService;
    private readonly IExplorer explorer;

    public ClipboardCutCommand(ICliboardService clipboardService, IExplorer explorer)
    {
        this.clipboardService = clipboardService;
        this.explorer = explorer;
    }

    public Task ExecuteAsync()
    {
        clipboardService.RegisterForCut(explorer.SelectedItem);
        return Task.CompletedTask;
    }
}
