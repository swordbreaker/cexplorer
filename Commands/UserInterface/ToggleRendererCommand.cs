namespace console_explorer.Commands.UserInterface
{
    internal record ToggleRendererCommand(IExplorer Explorer) : ICommand
    {
        public string Name => "Toggle Renderer";

        public Task ExecuteAsync()
        {
            Explorer.ToggleRenderer();
            return Task.CompletedTask;
        }
    }
}
