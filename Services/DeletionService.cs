using console_explorer.Factories;
using Spectre.Console;

namespace console_explorer.Services
{
    public class DeletionService : IDeletionService
    {
        private readonly ICommandManager commandManager;
        private readonly ICommandFactory commandFactory;
        private readonly IExplorerUi explorerUi;

        public DeletionService(ICommandManager commandManager, ICommandFactory commandFactory, IExplorerUi explorerUi)
        {
            this.commandManager = commandManager;
            this.commandFactory = commandFactory;
            this.explorerUi = explorerUi;
        }

        private FileSystemInfo? itemToDelete;

        public void RequestDeletion(FileSystemInfo item)
        {
            itemToDelete = item;
            explorerUi.UpdateStatus(new Markup($"[yellow]Are you sure to delete {item.Name}? y or n[/]"));
            explorerUi.OnKeyPressed += OnKeyPressed;
        }

        private void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Y)
            {
                Delete();
            }
            else if (keyInfo.Key == ConsoleKey.N)
            {
                CancelDeletion();
            }
        }

        public void CancelDeletion()
        {
            if (itemToDelete is not null)
            {
                explorerUi.UpdateStatus(new Markup($"[green]Directory deleted {itemToDelete.Name}.[/]"));
                itemToDelete = null;
                explorerUi.OnKeyPressed -= OnKeyPressed;
            }
        }

        public void Delete()
        {
            if (itemToDelete is not null)
            {
                commandManager.Execute(commandFactory.CreateDeleteCommand(itemToDelete));
                explorerUi.UpdateStatus(new Markup($"[green]File deleted {itemToDelete.Name}.[/]"));
                explorerUi.OnKeyPressed -= OnKeyPressed;
            }
        }
    }
}
