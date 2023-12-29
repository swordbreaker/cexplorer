namespace console_explorer.Commands;

internal interface IUndoableCommand : ICommand
{
    Task UndoAsync();
}
