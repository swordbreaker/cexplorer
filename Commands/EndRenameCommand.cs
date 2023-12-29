namespace console_explorer.Commands;

public class EndRenameCommand : IUndoableCommand
{
    private readonly FileSystemInfo selectedItem;
    private readonly string newName;
    private string oldName;

    public EndRenameCommand(FileSystemInfo selectedItem, string newName)
    {
        this.selectedItem = selectedItem;
        this.newName = newName;
    }

    public string Name => "End rename";

    public Task ExecuteAsync()
    {
        this.oldName = selectedItem.FullName;
        if (selectedItem is DirectoryInfo d)
        {
            d.MoveTo(Path.Combine(d.Parent!.FullName, newName));
        }
        else if (selectedItem is FileInfo f)
        {
            f.MoveTo(Path.Combine(f.DirectoryName!, newName));
        }
        return Task.CompletedTask;
    }

    public Task UndoAsync()
    {
        if (selectedItem is DirectoryInfo d)
        {
            d.MoveTo(oldName);
        }
        else if (selectedItem is FileInfo f)
        {
            f.MoveTo(oldName);
        }
        return Task.CompletedTask;
    }
}
