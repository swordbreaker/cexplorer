namespace console_explorer.Commands;

public class ClipboardCutCommand : IUndoableCommand
{
    public string Name => "Cut to clipboard";

    public Task ExecuteAsync()
    {
        // TODO
        return Task.CompletedTask;
    }

    public Task UndoAsync()
    {
        // TODO
        return Task.CompletedTask;
    }
}
