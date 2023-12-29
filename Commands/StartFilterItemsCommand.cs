namespace console_explorer.Commands
{
    public record StartFilterItemsCommand(IExplorer Explorer) : ICommand
    {
        public string Name => "Filter Items";

        public Task ExecuteAsync()
        {
            Explorer.StartFiltering();
            return Task.CompletedTask;
        }
    }
}
