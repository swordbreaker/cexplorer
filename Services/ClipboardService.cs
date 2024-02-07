using System.Drawing;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using Clowd.Clipboard;
using console_explorer.Factories;
using console_explorer.Services;

public class ClipboadService : ICliboardService
{
    private readonly IMoveCommandFactory moveCommandFactory;
    private readonly ICommandManager commandManager;

    private FileSystemInfo? cutItem;

    public ClipboadService(IMoveCommandFactory moveCommandFactory, ICommandManager commandManager)
    {
        this.moveCommandFactory = moveCommandFactory;
        this.commandManager = commandManager;
    }

    public async Task RegisterForCut(FileSystemInfo selectedItem)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // TODO Handle other platforms
            return;
        }

        cutItem = selectedItem;
        using var handle = await ClipboardGdi.OpenAsync();
        handle.SetText(selectedItem.FullName);
    }

    public async Task<(string[] PastedItems, string[] CuttedForm)> PasteAsync(DirectoryInfo workingDirectory)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // TODO Handle other platforms
            return (Array.Empty<string>(), Array.Empty<string>());
        }

        if(cutItem is not null)
        {
            var destination = Path.Combine(workingDirectory.FullName, cutItem.Name);
            var moveCommand = moveCommandFactory.Create(cutItem, destination);
            commandManager.Execute(moveCommand);
            cutItem = null;
            return (Array.Empty<string>(), Array.Empty<string>());
        }

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
            return (new[] { path }, Array.Empty<string>());
        }
        else if (handle.ContainsText())
        {
            var text = handle.GetText();
            if (text == cutItem?.FullName)
            {
                var destination = Path.Combine(workingDirectory.FullName, cutItem.Name);
                var sourceDestination  = cutItem.FullName;
                var moveCommand = moveCommandFactory.Create(cutItem, destination);
                commandManager.Execute(moveCommand);
                cutItem = null;
                return (new[] { destination }, new[] { sourceDestination });
            }
            else
            {
                var path = Path.Combine(workingDirectory.FullName, "text.txt");
                File.WriteAllText(path, text);
                return (new[] { path }, Array.Empty<string>());
            }
        }
        if (handle.ContainsFileDropList())
        {
            var files = handle.GetFileDropList();
            var createdItems = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                string? file = files[i];
                var path = Path.Combine(workingDirectory.FullName, file);
                File.Copy(file, path);
                createdItems[i] = path;
            }
            return (createdItems, Array.Empty<string>());
        }
        else
        {
            // TODO: Handle other types
            return (Array.Empty<string>(), Array.Empty<string>());
        }
    }

    public async Task CopyAsync(FileSystemInfo selectedItem)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // TODO Handle other platforms
            return;
        }

        cutItem = null;
        using var handle = await ClipboardGdi.OpenAsync();

        if (selectedItem.Extension is ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif")
        {
            var image = (Bitmap)Image.FromFile(selectedItem.FullName);
            handle.SetImage(image);
        }
        else if (selectedItem.Extension == ".txt")
        {
            var text = File.ReadAllText(selectedItem.FullName);
            handle.SetText(text);
        }
    }
}