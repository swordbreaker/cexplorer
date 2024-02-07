using console_explorer.UiElements;
using Spectre.Console;

namespace console_explorer.Services
{
    public class FilterService : IFilterService
    {
        private readonly IExplorerUi explorerUi;
        private readonly IExplorer explorer;

        public bool IsFiltering { get; private set; }

        public FilterService(IExplorerUi explorerUi, IExplorer explorer)
        {
            this.explorerUi = explorerUi;
            this.explorer = explorer;
        }

        public void Start()
        {
            IsFiltering = true;
            var input = new CInput(explorerUi);
            input.OnChanged += OnChanged;
            input.OnCanceled += OnCanceled;
            input.OnSubmitted += OnSubmitted;
            input.OnTab += OnTab;
            explorer.Filter = "";

            var gird = new Grid();
            gird.AddColumns(2);
            gird.AddRow(new Markup($"Filter by: "), input);
            explorerUi.UpdateStatus(gird);

            void OnChanged(string filter, string oldFilter)
            {
                explorer.Filter = filter;
            }

            void OnCanceled(string filter)
            {
                explorer.Filter = string.Empty;
                explorerUi.UpdateStatus(new Markup($"[yellow bold]Cancel filtering.[/]"));
                Dispose();
            }

            void OnSubmitted(string filter)
            {
                explorerUi.UpdateStatus(new Markup($"[green bold]Filtered by {filter}[/]"));
                Dispose();
            }

            void OnTab(CInput input)
            {
                var filteredItems = explorer.AllItems
                    .Where(i => i.Name.StartsWith(input.Text, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (filteredItems.Count == 0)
                {
                    return;
                }

                input.Text = filteredItems.First().Name;
            }

            void Dispose()
            {
                input.OnChanged -= OnChanged;
                input.OnCanceled -= OnCanceled;
                input.OnTab -= OnTab;
                input.OnSubmitted -= OnSubmitted;
                input.Dispose();
                IsFiltering = false;
            }
        }
    }
}
