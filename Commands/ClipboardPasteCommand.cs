using Clowd.Clipboard;
using System.Runtime.InteropServices;

namespace console_explorer.Commands;

public class ClipboardPasteCommand : IUndoableCommand
{
    private readonly IExplorer explorer;
    private string[] createdItems = Array.Empty<string>();

    public ClipboardPasteCommand(IExplorer explorer)
    {
        this.explorer = explorer;
    }

    public string Name => "Paste from clipboard";

    public async Task ExecuteAsync()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // TODO Handle other platforms
            return;
        }
        var workingDirectory = explorer.WorkingDirectory;

        using var handle = await ClipboardGdi.OpenAsync();
        if (handle.ContainsImage())
        {
            var image = handle.GetImage();
            var path = Path.Combine(workingDirectory.FullName, $"clipboard-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png");
            if (File.Exists(path))
            {
                path = Path.Combine(workingDirectory.FullName, $"clipboard-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png");
            }
            image.Save(path);
            createdItems = new[] { path };
        }
        else if (handle.ContainsText())
        {
            var text = handle.GetText();
            var path = Path.Combine(workingDirectory.FullName, "text.txt");
            File.WriteAllText(path, text);
            createdItems = new[] { path };
        }
        if (handle.ContainsFileDropList())
        {
            var files = handle.GetFileDropList();
            createdItems = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                string? file = files[i];
                var path = Path.Combine(workingDirectory.FullName, file);
                File.Copy(file, path);
                createdItems[i] = path;
            }
        }
        else
        {
            // TODO: Handle other types
            createdItems = Array.Empty<string>();
        }
    }

    public Task UndoAsync()
    {
        foreach (var item in createdItems)
        {
            File.Delete(item);
        }
        return Task.CompletedTask;
    }
}
