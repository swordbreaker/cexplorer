using console_explorer.Services;
using Spectre.Console;

namespace console_explorer.Commands;

public record HelpCommand(ICommandManager CommandManager, IExplorerUi ExplorerUi) : ICommand
{
    public string Name => "Show help";

    public Task ExecuteAsync()
    {
        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();

        CommandManager.GetCommandHelp
            .ToList()
            .ForEach(x => grid.AddRow($"[green bold]{x.key}[/]", x.commandName));

        ExplorerUi.UpdateRight(grid, "Help");
        return Task.CompletedTask;
    }
}
