using console_explorer.Services;

namespace console_explorer.Commands;

public record RedoCommand(ICommandManager commandManager) : ICommand
{
    public string Name => "Redo";

    public Task ExecuteAsync()
    {
        this.commandManager.Redo();
        return Task.CompletedTask;
    }
}
