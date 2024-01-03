using console_explorer.Commands;

namespace console_explorer.Factories
{
    public interface IMoveCommandFactory
    {
        IUndoableCommand Create(FileSystemInfo itemToMove, string destination);
    }
}