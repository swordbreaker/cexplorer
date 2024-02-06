using System.Collections.Concurrent;
using System.Diagnostics;
using Spectre.Console;
using Spectre.Console.Rendering;

public class ExplorerUi : IExplorerUi
{
    private ConcurrentQueue<Action<Layout>> renderQueue = new();
    private bool isActive = false;
    private SemaphoreSlim renderLock = new SemaphoreSlim(1, 1);
    private (int W, int H) Size;
    private bool isMarkedDirty = false;

    public event Action<ConsoleKeyInfo> OnKeyPressed;

    public int LeftRatio { get; private set; } = 3;
    public int RightRatio { get; private set; }  = 3;

    public async Task Render()
    {
        await renderLock.WaitAsync();
        isActive = true;

        try
        {
            // Create the layout
            var layout = CreateLayout();
            AnsiConsole.Write(layout);

            await AnsiConsole
                .Live(layout)
                .AutoClear(true)
                .StartAsync(async ctx =>
                {
                    while (isActive)
                    {
                        var isDirty = isMarkedDirty;
                        isMarkedDirty = false;

                        while (!renderQueue.IsEmpty)
                        {
                            isDirty = true;
                            if (renderQueue.TryDequeue(out var action))
                            {
                                action?.Invoke(layout);
                            }
                        }

                        try
                        {
                            if (Size != (Console.WindowWidth, Console.WindowHeight))
                            {
                                Size = (Console.WindowWidth, Console.WindowHeight);
                                isDirty = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }

                        if (isDirty)
                        {
                            ctx.Refresh();
                        }

                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey();
                            OnKeyPressed?.Invoke(key);
                        }
                        else
                        {
                            await Task.Delay(10);
                        }
                    }
                });
        }
        finally
        {
            isActive = false;
            renderLock.Release();
        }
    }

    public async Task StopAsync()
    {
        isActive = false;
        await renderLock.WaitAsync();
        renderLock.Release();
    }

    public void MarkAsDirty()
    {
        isMarkedDirty = true;
    }

    private Layout CreateLayout()
    {
        // Create the layout
        var layout = new Layout("Root")
            .SplitRows(
                new Layout()
                    .Ratio(5)
                    .SplitColumns(
                        new Layout(name: "Left").Ratio(LeftRatio),
                        new Layout(name: "Right").Ratio(RightRatio)),
                    new Layout("Bottom").Ratio(1));
        return layout;
    }

    public void UpdateLeft(IRenderable content, string title)
    {
        var panel = new Panel(content)
            .Expand();
        panel.Header = new PanelHeader(title.EscapeMarkup());

        renderQueue.Enqueue(layout =>
        {
            layout["Left"].Update(panel);
        });
    }

    public void UpdateRight(IRenderable renderable, string title)
    {
        var panel = new Panel(renderable).Expand();
        panel.Header = new PanelHeader(title.EscapeMarkup());
        renderQueue.Enqueue(layout =>
        {
            layout["Right"].Update(panel);
        });
    }

    public void UpdateStatus(IRenderable renderable)
    {
        var panel = new Panel(renderable).Expand();
        renderQueue.Enqueue(layout =>
        {
            layout["Bottom"].Update(panel);
        });
    }

    public void UpdateRightRatio(int ratio)
    {
        RightRatio = ratio;
        renderQueue.Enqueue(layout =>
        {
            if(ratio == 0)
            {
                layout["Right"].IsVisible = false;
                return;
            }
            
            layout["Right"].IsVisible = true;
            layout["Right"].Ratio(ratio);
        });
    }

    public void Dispose()
    {
        isActive = false;
        renderQueue.Clear();
    }
}