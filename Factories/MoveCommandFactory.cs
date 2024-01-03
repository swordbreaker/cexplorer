using console_explorer.Commands;
using Microsoft.Extensions.Logging;

namespace console_explorer.Factories;

public class MoveCommandFactory : IMoveCommandFactory
{
    private readonly IExplorerUi explorerUi;
    private readonly ILogger<MoveCommand> logger;

    public MoveCommandFactory(IExplorerUi explorerUi, ILogger<MoveCommand> logger)
    {
        this.explorerUi = explorerUi;
        this.logger = logger;
    }

    public IUndoableCommand Create(FileSystemInfo itemToMove, string destination) =>
        new MoveCommand(itemToMove, destination, explorerUi, logger);
}
