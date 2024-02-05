using console_explorer.Commands;
using console_explorer.ExtensionMethods;
using console_explorer.Factories;

namespace console_explorer.Services;

public class CommandManager : ICommandManager
{
    private readonly Stack<IUndoableCommand> undoStack = new();
    private readonly Stack<IUndoableCommand> redoStack = new();
    private readonly Dictionary<ConsoleKeyInfo, Func<ICommand>> commands;
    private readonly Dictionary<char, Func<ICommand>> charCommands;
    private readonly ICommandFactory commandFactory;
    public event Action<ICommand>? OnCommandExecuted;

    public CommandManager(ICommandFactory commandFactory)
    {
        this.commandFactory = commandFactory;

        commands = new()
        {
            [new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false)] = commandFactory.CreateSelectPreviousItemCommand,
            [new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false)] = commandFactory.CreateSelectNextItemCommand,
            [new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false)] = commandFactory.Create<ExecuteCurrentItemCommand>,
            [new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false)] = commandFactory.Create<GoBackADirectoryCommand>,
            [new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false)] = commandFactory.Create<GoToFirstItemCommand>,
            [new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false)] = commandFactory.Create<GoToLastItemCommand>,
            [new ConsoleKeyInfo('\u001a', ConsoleKey.Z, false, false, true)] = commandFactory.Create<UndoCommand>,
            [new ConsoleKeyInfo('\u0019', ConsoleKey.Y, false, false, true)] = commandFactory.Create<RedoCommand>,
            [new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false)] = commandFactory.Create<RequestDeletionCommand>,
            [new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false)] = commandFactory.Create<ToggleRendererCommand>,
            //[new ConsoleKeyInfo('?', , false, false, false)] = () => new HelpCommand(explorer),
            //[new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false)] = new MoveCommand(Explorer),
            //[new ConsoleKeyInfo('u', ConsoleKey.U, false, false, false)] = new UndoCommand(Explorer),
            //[new ConsoleKeyInfo('z', ConsoleKey.Z, false, false, false)] = new RedoCommand(Explorer),
            //[new ConsoleKeyInfo('h', ConsoleKey.H, false, false, false)] = new HelpCommand(Explorer),
            //[new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false)] = new SearchCommand(Explorer),
            //[new ConsoleKeyInfo('e', ConsoleKey.E, false, false, false)] = new EditCommand(Explorer),
            //[new ConsoleKeyInfo('t', ConsoleKey.T, false, false, false)] = new TouchCommand(Explorer),
            //[new ConsoleKeyInfo('f', ConsoleKey.F, false, false, false)] = new FindCommand(Explorer),
            //[new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false)] = new AboutCommand(Explorer),
            //[new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false)] = new OpenCommand(Explorer),
        };

        charCommands = new()
        {
            ['q'] = commandFactory.Create<QuitCommand>,
            ['c'] = commandFactory.Create<ClipboardCopyCommand>,
            ['p'] = commandFactory.Create<ClipboardPasteCommand>,
            ['x'] = commandFactory.Create<ClipboardCutCommand>,
            ['r'] = commandFactory.Create<StartRenameCommand>,
            ['l'] = commandFactory.Create<CalculateSizeCommand>,
            ['/'] = commandFactory.Create<StartFilterItemsCommand>,
            ['+'] = commandFactory.Create<TogglePreviewPanelSizeCommand>,
            ['?'] = commandFactory.Create<HelpCommand>,
            ['v'] = commandFactory.Create<ClipboardPasteCommand>,
            ['c'] = commandFactory.Create<ClipboardCopyCommand>,
            ['s'] = commandFactory.Create<ToggleSortOrderCommand>,
            ['e'] = commandFactory.Create<OpenExplorerCommand>,
            // TODO Move command
            // TODO Cut command
        };
    }

    public IEnumerable<(string key, string commandName)> GetCommandHelp =>
        commands
            .Select(x => (x.Key.ToDisplayString(), x.Value().Name))
            .Concat(charCommands
                .Select(x => (x.Key.ToString(), x.Value().Name)));

    public void Execute(ConsoleKeyInfo key)
    {
        if (commands.TryGetValue(key, out var command))
        {
            Execute(command());
            return;
        }

        if(charCommands.TryGetValue(key.KeyChar, out var charCommand))
        {
            Execute(charCommand());
            return;
        }
    }

    public void Execute<T>() where T : ICommand =>
        Execute(commandFactory.Create<T>());

    public void Execute(ICommand command)
    {
        command.ExecuteAsync();
        if (command is IUndoableCommand undoableCommand)
        {
            undoStack.Push(undoableCommand);
        }
        redoStack.Clear();
        OnCommandExecuted?.Invoke(command);
    }

    public void Undo()
    {
        if (undoStack.Count == 0)
        {
            return;
        }

        var command = undoStack.Pop();
        command.UndoAsync();
        redoStack.Push(command);
    }

    public void Redo()
    {
        if (redoStack.Count == 0)
        {
            return;
        }

        var command = redoStack.Pop();
        command.ExecuteAsync();
        undoStack.Push(command);
    }
}
