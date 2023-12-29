namespace console_explorer.Commands;

public record DeleteCommand : IUndoableCommand
{
    private readonly FileSystemInfo file;
    private FileSystemInfo removedItem;
    private string oldFilePath;

    private static Lazy<DirectoryInfo> TempDirectory =
        new(() => Directory.CreateTempSubdirectory("console-explorer"));

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
            f.MoveTo(TempDirectory.Value.FullName);
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
