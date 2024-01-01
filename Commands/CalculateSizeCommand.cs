using console_explorer.ExtensionMethods;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace console_explorer.Commands
{
    public class CalculateSizeCommand : ICommand
    {
        private readonly IExplorer explorer;
        private readonly IExplorerUi explorerUi;
        private readonly ILogger<CalculateSizeCommand> logger;

        public CalculateSizeCommand(IExplorer explorer, IExplorerUi explorerUi, ILogger<CalculateSizeCommand> logger)
        {
            this.explorer = explorer;
            this.explorerUi = explorerUi;
            this.logger = logger;
        }

        public string Name => "Calculate Folder Size";

        public async Task ExecuteAsync()
        {
            try
            {
                var list = new List<ProgressColumn> {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn()
                };

                var progressTask = new ProgressTask(0, "Test", 1);

                if (explorer.SelectedItem is DirectoryInfo d)
                {
                    await Task.Run(async () =>
                    {
                        var progress = new Progress<float>(x =>
                        {
                            progressTask.Value = x;
                            var grid = new Grid();
                            grid.AddColumns(list.Count);
                            var renderable = list.Select(x => x.Render(null, progressTask, TimeSpan.Zero));
                            grid.AddRow(renderable.ToArray());
                            explorerUi.UpdateStatus(grid.Expand());
                        });

                        var result = await Task.Run(() => d.CalculateSize(progress));
                        await Task.Delay(100);
                        explorerUi.UpdateStatus(new Markup($"[green bold]Size: {result.ToDisplayString()}[/]"));
                    });
                }
                else if(explorer.SelectedItem is FileInfo f)
                {
                    explorerUi.UpdateStatus(new Markup($"[green bold]Size: {new Bytes(f.Length).ToDisplayString()}[/]"));
                }
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Error occurred while calculating folder size");
                explorerUi.UpdateStatus(new Markup($"[bold red] {e.Message} [/]"));
            }
        }
    }
}
