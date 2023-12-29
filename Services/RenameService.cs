using console_explorer.Commands;
using console_explorer.UiElements;
using Spectre.Console;

namespace console_explorer.Services
{
    public class RenameService : IRenameService
    {
        private readonly IExplorerUi consoleUi;
        private readonly ICommandManager commandManager;

        private CInput input;

        public bool IsRenaming { get; private set; }

        public RenameService(IExplorerUi consoleUi, ICommandManager commandManager)
        {
            this.consoleUi = consoleUi;
            this.commandManager = commandManager;
        }

        public void Rename(FileSystemInfo selectedItem)
        {
            IsRenaming = true;
            input = new(consoleUi);
            input.OnSubmitted += OnSubmitted;
            input.OnCanceled += OnCanceled;
            input.OnTab += OnTab;
            var grid = new Grid();
            grid.AddColumns(2);
            grid.AddRow(new Markup($"Rename {selectedItem.Name} to: "), input);
            consoleUi.UpdateStatus(grid);

            void OnSubmitted(string newName)
            {
                if (newName.Length == 0)
                {
                    consoleUi.UpdateStatus(new Markup($"[red bold]Name cannot be empty[/]"));
                    return;
                }
                if (selectedItem is FileInfo file && file.DirectoryName is null)
                {
                    consoleUi.UpdateStatus(new Markup($"[red bold]Cannot rename root directory[/]"));
                    return;
                }
                if (selectedItem is DirectoryInfo directory && directory.Parent is null)
                {
                    consoleUi.UpdateStatus(new Markup($"[red bold]Cannot rename root directory[/]"));
                    return;
                }

                try
                {
                    consoleUi.UpdateStatus(new Markup($"[green bold]Renamed {selectedItem.Name} to {newName}[/]"));
                    commandManager.Execute(new EndRenameCommand(selectedItem, newName));
                }
                catch (Exception ex)
                {
                    consoleUi.UpdateStatus(new Markup($"[red bold]Error renaming {selectedItem.Name} to {newName}: {ex.Message}[/]"));
                }
                finally
                {
                    IsRenaming = false;
                    Dispose();
                }
            }

            void OnCanceled(string newName)
            {
                IsRenaming = false;
                consoleUi.UpdateStatus(new Markup($"[yellow bold]Rename canceled.[/]"));
                IsRenaming = false;
                Dispose();
            }

            void OnTab(CInput input)
            {
                if(input.Text.Length == 0)
                {
                    var name = selectedItem.Name.Replace(selectedItem.Extension, string.Empty);
                    input.Text += name;
                }
                else if(input.CursorPosition == input.Text.Length)
                {
                    input.Text += selectedItem.Extension;
                }
            }

            void Dispose()
            {
                input.OnSubmitted -= OnSubmitted;
                input.OnCanceled -= OnCanceled;
                input.OnTab -= OnTab;
                input.Dispose();
                IsRenaming = false;
            }
        }
    }
}
