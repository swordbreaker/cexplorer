namespace console_explorer.Commands.UserInterface
{
    public class OpenExplorerCommand(IExplorer Explorer) : ICommand
    {
        public string Name => "Open Explorer";

        public Task ExecuteAsync()
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "explorer.exe";
            process.StartInfo.Arguments = Explorer.SelectedItem switch
            {
                DirectoryInfo directory => directory.FullName,
                FileInfo file => file.DirectoryName,
                _ => throw new NotImplementedException(),
            };
            process.Start();
            return Task.CompletedTask;
        }
    }
}
