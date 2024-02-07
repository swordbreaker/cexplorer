using console_explorer.Factories;
using console_explorer.UiElements;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace console_explorer.Services
{
    public class CreateItemService(
        IExplorerUi explorerUi,
        IExplorer explorer,
        ICommandFactory commandFactory,
        ICommandManager commandManager,
        ILogger<CreateItemService> logger) : ICreateItemService
    {
        private CInput input;

        public void CreateItem()
        {
            explorer.IsDisabled = true;
            input = new CInput(explorerUi);
            input.OnSubmitted += OnSubmitted;
            input.OnCanceled += OnCanceled;

            var grid = new Grid();
            grid.AddColumns(2);
            grid.AddRow(new Markup($"Name (For Directory add a slash (/) at the end):"), input);
            explorerUi.UpdateStatus(grid);
        }

        private void OnSubmitted(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                logger.LogError("Name cannot be empty");
                return;
            }

            var command = commandFactory.CreateCreateItemCommand(name);
            commandManager.Execute(command);
            Dispose();
        }

        private void OnCanceled(string name)
        {
            explorerUi.UpdateStatus(new Markup($"[yellow bold]Cancel item creation.[/]"));
            Dispose();
        }

        public void Dispose()
        {
            input.OnSubmitted -= OnSubmitted;
            input.OnCanceled -= OnCanceled;
            input.Dispose();
            explorer.IsDisabled = false;
        }
    }
}
