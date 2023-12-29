using Clowd.Clipboard;
using System.Drawing;
using System.Runtime.InteropServices;

namespace console_explorer.Commands;

public record ClipboardCopyCommand(IExplorer Explorer) : ICommand
{
    public string Name => "Copy to clipboard";

    public async Task ExecuteAsync()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // TODO Handle other platforms
            return;
        }

        using var handle = await ClipboardGdi.OpenAsync();
        var selectedItem = Explorer.SelectedItem;

        if (selectedItem.Extension is ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif")
        {
            var image = (Bitmap)Image.FromFile(selectedItem.FullName);
            handle.SetImage(image);
        }
        else
        {
            handle.SetFileDropList(new[] { selectedItem.FullName });
        }
    }
}
