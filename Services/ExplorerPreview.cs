using console_explorer.Services;
using Spectre.Console;
using System.Net.Http.Headers;
using System.Text;

public class ExplorerPreview : IExplorerPreview
{
    private readonly IExplorerUi consoleUi;

    public ExplorerPreview(IExplorerUi consoleUi)
    {
        this.consoleUi = consoleUi;
    }

    public void Show(FileSystemInfo item)
    {
        if (item is DirectoryInfo dir)
        {
            ShowDirectory(dir);
        }
        else if (item is FileInfo file)
        {
            ShowFile(file);
        }
    }

    private void ShowDirectory(DirectoryInfo dir)
    {
        var table = new Table();
        table.AddColumns("Name", "Size", "Last Modified");

        try
        {
            foreach (var d in dir.GetDirectories())
            {
                table.AddRow(d.Name.EscapeMarkup(), "NA", d.LastWriteTime.ToShortTimeString().EscapeMarkup());
            }
        }
        catch (UnauthorizedAccessException)
        {
            table.AddRow("[red]Access denied[/]", "NA", "NA");
        }

        try
        {
            foreach (var f in dir.GetFiles())
            {
                table.AddRow(f.Name.EscapeMarkup(), f.Length.ToString(), f.LastWriteTime.ToShortTimeString());
            }
        }
        catch (UnauthorizedAccessException)
        {
            table.AddRow("[red]Access denied[/]", "NA", "NA");
        }

        consoleUi.UpdateRight(table, dir.FullName);
    }

    private void ShowFile(FileInfo file)
    {
        try
        {
            ShowFileInternal(file);
        }
        catch(Exception e)
        {
            consoleUi.UpdateRight(new Markup($"[red]Error: {e.Message.EscapeMarkup()}[/]"), file.Name);
        }
    }

    private void ShowFileInternal(FileInfo file)
    {
        switch (file.Extension)
        {
            case ".png":
            case ".jpg":
            case ".jpeg":
            case ".gif":
                var image = new CanvasImage(file.FullName);
                consoleUi.UpdateRight(image, file.Name);
                break;
            case ".md":
            case ".txt":
            case ".cs":
            case ".json":
            case ".xml":
            case ".html":
            case ".htm":
            case ".js":
            case ".css":
            case ".cshtml":
            case ".csproj":
            case ".sln":
            case ".gitignore":
            case ".gitattributes":
            case ".config":
            case ".yaml":
            case ".yml":
            case ".vb":
            case ".vbproj":
            case ".vbhtml":
                {
                    using var steam = file.OpenRead();
                    using var reader = new StreamReader(steam, true);

                    var sb = new StringBuilder();
                    int lines = 0;
                    while (!reader.EndOfStream && lines++ < 50)
                    {
                        sb.AppendLine(reader.ReadLine()!.Replace("\t", "    "));
                    }

                    var text = new Text(sb.ToString())
                        .Overflow(Overflow.Fold);
                    consoleUi.UpdateRight(text, file.Name);
                    break;
                }
            default:
                {
                    var tx = new Markup($"[gray]No preview for {file.Extension} available.[/]");
                    consoleUi.UpdateRight(tx, file.Name);
                    break;
                }

        }
    }
}