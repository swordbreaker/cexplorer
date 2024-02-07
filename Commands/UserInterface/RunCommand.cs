using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace console_explorer.Commands.UserInterface
{
    public class RunCommand(IExplorer explorer, ILogger<RunCommand> logger) : ICommand
    {
        public string Name => "Run";

        public async Task ExecuteAsync()
        {
            var command = await explorer.Ask("Command: ");
            if (string.IsNullOrWhiteSpace(command))
            {
                logger.LogWarning("No command was entered");
                return;
            }

            var fileName = command.Split(' ').First();
            var arguments = command[fileName.Length..].Trim();
            var process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            process.OutputDataReceived += (sender, args) => logger.LogInformation(args.Data);
        }
    }
}
