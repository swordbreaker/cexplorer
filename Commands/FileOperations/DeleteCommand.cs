namespace console_explorer.Commands.FileOperations;

public record DeleteCommand : IUndoableCommand
{
    private readonly FileSystemInfo file;
    private FileSystemInfo removedItem;
    private string oldFilePath;

    private static Lazy<DirectoryInfo> TempDirectory =
        new(() =>
        {
            var dir = Directory.CreateTempSubdirectory("console-explorer");

            if (!dir.Exists)
            {
                dir.Create();
            }

            return dir;
        });

    public string Name => "Delete";

    public DeleteCommand(FileSystemInfo file)
    {
        this.file = file;
    }

    public Task ExecuteAsync()
    {
        if (file is DirectoryInfo d)
        {
            oldFilePath = d.FullName;
            d.MoveTo(TempDirectory.Value.FullName);
            removedItem = d;
        }
        else if (file is FileInfo f)
        {
            oldFilePath = f.FullName;
            var newFilePath = Path.Combine(TempDirectory.Value.FullName, f.Name);

            f.MoveTo(newFilePath, true);
            removedItem = f;
        }
        return Task.CompletedTask;
    }

    public Task UndoAsync()
    {
        if (removedItem is DirectoryInfo d)
        {
            d.MoveTo(oldFilePath);
        }
        else if (removedItem is FileInfo f)
        {
            f.MoveTo(oldFilePath);
        }
        return Task.CompletedTask;
    }
}
