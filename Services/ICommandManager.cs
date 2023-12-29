using console_explorer.Commands;

namespace console_explorer.Services;

public interface ICommandManager
{
    IEnumerable<(string key, string commandName)> GetCommandHelp { get; }

    event Action<ICommand>? OnCommandExecuted;

    void Execute(ConsoleKeyInfo key);
    void Execute(ICommand command);
    void Execute<T>() where T : ICommand;
    void Redo();
    void Undo();
}