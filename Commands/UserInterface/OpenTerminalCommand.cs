namespace console_explorer.Commands.UserInterface
{
    public class OpenTerminalCommand : ICommand
    {
        public string Name => "Open Terminal";

        public Task ExecuteAsync()
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.Start();
            return Task.CompletedTask;
        }
    }
}