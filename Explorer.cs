using console_explorer;
using console_explorer.Commands;
using console_explorer.Services;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Text.RegularExpressions;

public partial class Explorer : IExplorer
{
    private readonly IExplorerUi explorerUi;
    private readonly IExplorerPreview preview;
    private readonly IRenameService renameService;
    private readonly ICommandManager commandManager;
    private readonly IDeletionService deletionService;
    private readonly IFilterService filterService;
    private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
    private readonly ILogger<Explorer> logger;

    private IItemsRenderer itemsRenderer;
    private bool isDisposed = false;
    private int index = 0;
    private string filter = string.Empty;
    public SortOrder SortOrder { get; set; } = SortOrder.Default;

    public string Filter
    {
        get => filter;
        set
        {
            filter = value;
            UpdateUi();
        }
    }

    public DirectoryInfo WorkingDirectory { get; private set; }
    public FileSystemInfo SelectedItem { get; private set; }
    public int ItemCount => Items.Count() + 1;

    public int FileIndex
    {
        get => index;
        set
        {
            if (value == index) return;

            try
            {
                var n = ItemCount;
                index = value % n;
                if (index < 0)
                {
                    index = n + index;
                }
            }
            catch (Exception e)
            {
                // do nothing
                logger.LogError(e, "Error occurred while switching File Index");
            }
        }
    }

    public IEnumerable<FileSystemInfo> AllItems =>
        WorkingDirectory.GetDirectories()
        .OfType<FileSystemInfo>()
        .Concat(WorkingDirectory.GetFiles());

    public IEnumerable<FileSystemInfo> Items
    {
        get
        {
            var regex = string.IsNullOrEmpty(Filter)
                ? MatchAllRegex()
                : new Regex(Filter, RegexOptions.IgnoreCase);

            return WorkingDirectory.GetDirectories()
                .OfType<FileSystemInfo>()
                .Concat(WorkingDirectory.GetFiles())
                .Where(i => regex.IsMatch(i.Name));
        }
    }

    public Explorer(
        IExplorerUi explorerUi,
        IExplorerPreview explorerPreview,
        ICommandManager commandManager,
        IRenameService renameService,
        IDeletionService deletionService,
        ILogger<Explorer> logger)
    {
        this.explorerUi = explorerUi;
        this.WorkingDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        this.preview = explorerPreview;
        this.commandManager = commandManager;
        this.renameService = renameService;
        this.deletionService = deletionService;
        this.logger = logger;
        this.filterService = new FilterService(explorerUi, this);
        this.itemsRenderer = new TreeRenderer(explorerUi, 10);

        SelectedItem = WorkingDirectory;

        AnsiConsole.Console.Profile.Encoding = Console.Out.Encoding;
        AnsiConsole.Profile.Encoding = Console.Out.Encoding;

        explorerUi.OnKeyPressed += OnKeyPressed;
        commandManager.OnCommandExecuted += CommandManager_OnCommandExecuted;
    }

    private void CommandManager_OnCommandExecuted(ICommand command)
    {
        if (command is EndRenameCommand)
        {
            UpdateUi();
        }
    }

    public async Task StartAsync()
    {
        semaphoreSlim.Wait();
        UpdateUi();
        explorerUi.UpdateStatus(new Markup($"Press [green bold]?[/] for help"));
        explorerUi.Render();
        await semaphoreSlim.WaitAsync();
    }

    public void ExecuteSelectedItem()
    {
        if (SelectedItem is DirectoryInfo d)
        {
            EnterDirectory(d);
        }
        else if (SelectedItem is FileSystemInfo f)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = f.FullName,
                UseShellExecute = true
            };

            try
            {
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception e)
            {
                explorerUi.UpdateStatus(new Markup($"[red bold]Error opening {f.Name}: {e.Message}[/]"));
            }
        }
    }

    public void GoBackADirectory()
    {
        if (WorkingDirectory.Parent != null)
        {
            EnterDirectory(WorkingDirectory.Parent);
        }
    }

    public void EnterDirectory(DirectoryInfo directory)
    {
        var oldWorkingDirectory = WorkingDirectory;
        try
        {
            WorkingDirectory = directory;
            deletionService.CancelDeletion();
            index = 0;
            Filter = string.Empty;
            Directory.SetCurrentDirectory(WorkingDirectory.FullName);
            Environment.CurrentDirectory = WorkingDirectory.FullName;
        }
        catch (Exception e)
        {
            WorkingDirectory = oldWorkingDirectory;

            logger.LogError(e, "Error occurred while entering directory");
            explorerUi.UpdateStatus(new Markup($"[red bold]Error entering directory {directory.Name}: {e.Message}[/]"));
        }
    }

    public void StartRenameCurrent()
    {
        renameService.Rename(SelectedItem);
    }

    public void RequestDeleteCurrent()
    {
        deletionService.RequestDeletion(SelectedItem);
    }

    public void StartFiltering()
    {
        filterService.Start();
    }

    public void ToggleRenderer()
    {
        if (itemsRenderer is TreeRenderer)
        {
            itemsRenderer = new GridRenderer(explorerUi, 6);
        }
        else
        {
            itemsRenderer = new TreeRenderer(explorerUi, 10);
        }
        UpdateUi();
    }

    private void UpdateUi()
    {
        try
        {
            var items = Items
                .Order(new SortOderComparer(SortOrder))
                .Prepend(WorkingDirectory)
                .ToList();

            if(index >= items.Count)
            {
                index = items.Count - 1;
            }

            if(SelectedItem.Name != items[index].Name)
            {
                SelectedItem = items[index];
                preview.Show(SelectedItem);
            }

            itemsRenderer.RenderItems(items, index, $"Sorted by {SortOrder}");
        }
        catch (Exception e)
        {
            explorerUi.UpdateLeft(new Markup($"[red]Error: {e.Message.EscapeMarkup()}[/]"), "Error");
        }
    }

    private async void OnKeyPressed(ConsoleKeyInfo key)
    {
        if (renameService.IsRenaming || filterService.IsFiltering)
        {
            return;
        }

        commandManager.Execute(key);

        switch (key.Key)
        {
            case ConsoleKey.G:
                await explorerUi.StopAsync();
                AnsiConsole.Clear();
                AnsiConsole.Ask<string>("Goto: ");
                break;
            default:
                break;
        }

        UpdateUi();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;

        Console.Clear();
        Console.Out.Flush();
        Console.Out.Write(WorkingDirectory.FullName);
        semaphoreSlim.Release();
        semaphoreSlim.Dispose();
        explorerUi.Dispose();
    }

    [GeneratedRegex(".*", RegexOptions.IgnoreCase, "gsw-CH")]
    private static partial Regex MatchAllRegex();
}