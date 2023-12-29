using console_explorer.Commands;
using console_explorer.Factories;
using console_explorer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IExplorer, Explorer>();
builder.Services.AddSingleton<IExplorerUi, ExplorerUi>();
builder.Services.AddSingleton<IItemsRendererFactory, ItemsRendererFactory>();
builder.Services.AddSingleton<IExplorerPreview, ExplorerPreview>();
builder.Services.AddSingleton<ICommandManager, CommandManager>();
builder.Services.AddSingleton<IRenameService, RenameService>();
builder.Services.AddSingleton<ICommandFactory, CommandFactory>();
builder.Services.AddSingleton<IDeletionService, DeletionService>();

// Commands
builder.Services.AddTransient<StartRenameCommand>();
builder.Services.AddTransient<UndoCommand>();
builder.Services.AddTransient<RedoCommand>();
builder.Services.AddTransient<QuitCommand>();
builder.Services.AddTransient<DeleteCommand>();
builder.Services.AddTransient<GoBackADirectoryCommand>();
builder.Services.AddTransient<GoToFirstItemCommand>();
builder.Services.AddTransient<GoToLastItemCommand>();
builder.Services.AddTransient<ClipboardCopyCommand>();
builder.Services.AddTransient<ClipboardPasteCommand>();
builder.Services.AddTransient<ClipboardCutCommand>();
builder.Services.AddTransient<ExecuteCurrentItemCommand>();
builder.Services.AddTransient<RequestDeletionCommand>();
builder.Services.AddTransient<HelpCommand>();
builder.Services.AddTransient<CalculateSizeCommand>();
builder.Services.AddTransient<StartFilterItemsCommand>();
builder.Services.AddTransient<ToggleRendererCommand>();
builder.Services.AddTransient<TogglePreviewPanelSizeCommand>();

using IHost host = builder.Build();

await host.Services.GetRequiredService<IExplorer>().StartAsync();