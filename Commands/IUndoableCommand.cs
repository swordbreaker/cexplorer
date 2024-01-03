namespace console_explorer.Commands;

public interface IUndoableCommand : ICommand
{
    Task UndoAsync();
}
