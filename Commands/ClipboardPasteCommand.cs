using System.Runtime.InteropServices;

namespace console_explorer.Commands;

public class ClipboardPasteCommand : IUndoableCommand
{
    private readonly IExplorer explorer;
    private readonly ICliboardService clipboardService;
    private string[] createdItems = Array.Empty<string>();
    private string[] cuttedItems = Array.Empty<string>();

    public ClipboardPasteCommand(IExplorer explorer, ICliboardService clipboardService)
    {
        this.explorer = explorer;
        this.clipboardService = clipboardService;
    }

    public string Name => "Paste from clipboard";

    public async Task ExecuteAsync()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // TODO Handle other platforms
            return;
        }
        
        (createdItems, cuttedItems) = await clipboardService.PasteAsync(explorer.WorkingDirectory);
    }

    public Task UndoAsync()
    {
        if(createdItems.Length == createdItems.Length)
        {
            for (int i = 0; i < createdItems.Length; i++)
            {
                string src = createdItems[i];
                string dest = cuttedItems[i];

                if (Path.EndsInDirectorySeparator(src))
                {
                    Directory.Move(src, dest);
                }
                else
                {
                    File.Move(src, dest);
                }
            }
        }
        else
        {
            foreach (var item in createdItems)
            {
                if (Path.EndsInDirectorySeparator(item))
                {
                    Directory.Delete(item);
                }
                else
                {
                    File.Delete(item);
                }
            }
        }

        return Task.CompletedTask;
    }
}
