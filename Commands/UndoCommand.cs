using console_explorer.Services;

namespace console_explorer.Commands;

public record UndoCommand(ICommandManager commandManager) : ICommand
{
    public string Name => "Undo";

    public Task ExecuteAsync()
    {
        this.commandManager.Undo();
        return Task.CompletedTask;
    }
}
