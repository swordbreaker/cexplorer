
using console_explorer.Commands;

namespace console_explorer.Factories
{
    public interface ICommandFactory
    {
        ICommand Create<T>() where T : ICommand;
        ICommand CreateDeleteCommand(FileSystemInfo itemToDelete);
        ICommand CreateSelectNextItemCommand();
        ICommand CreateSelectPreviousItemCommand();
    }
}
