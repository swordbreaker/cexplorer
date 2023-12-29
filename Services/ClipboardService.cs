using System.Drawing;
using System.Runtime.InteropServices;
using Clowd.Clipboard;

public class ClipboadService : ICliboardService
{
    public async Task PasteAsync(DirectoryInfo workingDirectory)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // TODO Handle other platforms
            return;
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
        }
        else if (handle.ContainsText())
        {
            var text = handle.GetText();
            var path = Path.Combine(workingDirectory.FullName, "text.txt");
            File.WriteAllText(path, text);
        }
        else
        {
            // TODO: Handle other types
        }
    }

    public async Task CopyAsync(FileSystemInfo selectedItem)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // TODO Handle other platforms
            return;
        }

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