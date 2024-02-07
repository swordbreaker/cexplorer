using console_explorer.Commands;

namespace console_explorer.Services;

public interface ICommandManager
{
    IEnumerable<(string key, string commandName)> GetCommandHelp { get; }

    event Action<ICommand>? OnCommandExecuted;
    Task Execute(ConsoleKeyInfo key);
    Task Execute(ICommand command);
    Task Execute<T>() where T : ICommand;
    Task Redo();
    Task Undo();
}