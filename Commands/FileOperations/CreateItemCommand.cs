using Microsoft.Extensions.Logging;

namespace console_explorer.Commands.FileOperations;

public class CreateItemCommand(string itemName, IExplorer explorer, ILogger<CreateItemCommand> logger) : IUndoableCommand
{
    private readonly string itemName = itemName;
    private readonly IExplorer explorer = explorer;
    private FileSystemInfo? createdItem;

    public string Name => "Create a file or folder";

    public Task ExecuteAsync()
    {
        if (Path.EndsInDirectorySeparator(this.itemName))
        {
            this.createdItem = explorer.WorkingDirectory.CreateSubdirectory(this.itemName);
            logger.LogInformation("Created folder {0}", this.createdItem.FullName);
        }
        else
        {
            var file = new FileInfo(Path.Combine(explorer.WorkingDirectory.FullName, this.itemName));
            if (file.Exists)
            {
                logger.LogWarning("File {0} already exists", file.FullName);
                return Task.CompletedTask;
            }

            file.Create().Close();
            this.createdItem = file;
            logger.LogInformation("Created file {0}", this.createdItem.FullName);
        }

        return Task.CompletedTask;
    }

    public Task UndoAsync()
    {
        if (this.createdItem is DirectoryInfo dir && dir.GetFiles().Length == 0)
        {
            this.createdItem.Delete();
            logger.LogInformation("Deleted folder {0}", this.createdItem.FullName);
        }
        else if(this.createdItem is FileInfo file)
        {
            this.createdItem.Delete();
            logger.LogInformation("Deleted file {0}", this.createdItem.FullName);
        }
        else
        {
            throw new InvalidOperationException("Cannot undo create folder operation because the folder is not empty.");
        }
        return Task.CompletedTask;
    }
}
