using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace console_explorer.Commands;

public class MoveCommand : IUndoableCommand
{
    private readonly FileSystemInfo itemToMove;
    private readonly string destination;
    private readonly IExplorerUi explorerUi;
    private readonly ILogger<MoveCommand> logger;
    private string oldDestination;

    public MoveCommand(FileSystemInfo itemToMove, string destination, IExplorerUi explorerUi, ILogger<MoveCommand> logger)
    {
        this.itemToMove = itemToMove;
        this.destination = destination;
        this.explorerUi = explorerUi;
        this.logger = logger;
    }

    public string Name => "Move file or directory";

    public Task ExecuteAsync()
    {
        oldDestination = itemToMove.FullName;

        try
        {
            switch (itemToMove)
            {
                case FileInfo file:
                    file.MoveTo(destination);
                    break;
                case DirectoryInfo directory:
                    directory.MoveTo(destination);
                    break;
            }
        }
        catch (Exception ex)
        {
            explorerUi.UpdateStatus(new Markup($"[bold red]{ex.Message.EscapeMarkup()} [/]"));
            logger.LogError(ex, "Failed to move {ItemToMove} to {Destination}", itemToMove, destination);
        }

        return Task.CompletedTask;
    }

    public Task UndoAsync()
    {
        if (oldDestination is not null)
        {
            switch (itemToMove)
            {
                case FileInfo file:
                    file.MoveTo(oldDestination);
                    break;
                case DirectoryInfo directory:
                    directory.MoveTo(oldDestination);
                    break;
            }
        }

        return Task.CompletedTask;
    }
}
