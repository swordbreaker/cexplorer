using Spectre.Console;
using Spectre.Console.Rendering;
using System.Text;

namespace console_explorer.UiElements
{
    public class CInput : IRenderable, IDisposable
    {
        private readonly IExplorerUi explorerUi;
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private Markup markup;
        private bool isActive = true;
        private bool isDisposed;

        public event Action<string> OnSubmitted;
        public event Action<string> OnCanceled;
        public event Action<string, string> OnChanged;
        public event Action<CInput> OnDirty;
        public event Action<CInput> OnTab;

        public string Text
        {
            get => stringBuilder.ToString();
            set
            {
                var old = stringBuilder.ToString();
                stringBuilder.Clear();
                stringBuilder.Append(value);
                SetCursorPosition(stringBuilder.Length);
                Update();
                OnChanged?.Invoke(stringBuilder.ToString(), old);
            }
        }

        public int CursorPosition { get; private set; } = 0;

        public CInput(IExplorerUi explorerUi)
        {
            this.explorerUi = explorerUi;
            markup = new Markup(stringBuilder.ToString());
            isActive = true;
            explorerUi.OnKeyPressed += OnKeyPressed;
            Update();
        }

        private void OnKeyPressed(ConsoleKeyInfo info)
        {
            switch (info.Key)
            {
                case ConsoleKey.Enter:
                    OnSubmitted?.Invoke(stringBuilder.ToString());
                    break;
                case ConsoleKey.Escape:
                    OnCanceled?.Invoke(stringBuilder.ToString());
                    break;
                case ConsoleKey.Backspace:
                    if (stringBuilder.Length > 0)
                    {
                        var old = stringBuilder.ToString();
                        stringBuilder.Remove(CursorPosition - 1, 1);
                        SetCursorPosition(CursorPosition - 1);
                        Update();
                        OnChanged?.Invoke(stringBuilder.ToString(), old);
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    {
                        SetCursorPosition(CursorPosition - 1);
                        Update();
                    }
                    break;
                case ConsoleKey.RightArrow:
                    {
                        SetCursorPosition(CursorPosition + 1);
                        Update();
                    }
                    break;
                case ConsoleKey.Delete when info.Modifiers.HasFlag(ConsoleModifiers.Control):
                    {
                        var old = stringBuilder.ToString();
                        stringBuilder.Clear();
                        SetCursorPosition(0);
                        Update();
                        OnChanged?.Invoke(stringBuilder.ToString(), old);
                    }
                    break;
                case ConsoleKey.Delete:
                    {
                        if (CursorPosition < stringBuilder.Length)
                        {
                            var old = stringBuilder.ToString();
                            stringBuilder.Remove(CursorPosition, 1);
                            Update();
                            OnChanged?.Invoke(stringBuilder.ToString(), old);
                        }
                    }
                    break;
                case ConsoleKey.Home:
                    {
                        SetCursorPosition(0);
                        Update();
                    }
                    break;
                case ConsoleKey.End:
                    {
                        SetCursorPosition(stringBuilder.Length);
                        Update();
                    }
                    break;
                case ConsoleKey.Tab:
                    {
                        OnTab?.Invoke(this);
                    }
                    break;
                case { } when !char.IsControl(info.KeyChar):
                    {
                        var old = stringBuilder.ToString();
                        stringBuilder.Insert(CursorPosition, info.KeyChar);
                        SetCursorPosition(CursorPosition + 1);
                        Update();
                        OnChanged?.Invoke(stringBuilder.ToString(), old);
                    }
                    break;
                default:
                    // do noting
                    break;
            }
        }

        public void Dispose()
        {
            if (!isDisposed) return;

            isDisposed = true;
            explorerUi.OnKeyPressed -= OnKeyPressed;
            isActive = false;
        }

        private void SetCursorPosition(int position)
        {
            if (position < 0 || position > stringBuilder.Length) return;
            CursorPosition = position;
        }

        private void Update()
        {
            var userInput = stringBuilder.ToString().EscapeMarkup().Insert(CursorPosition, "[blue]|[/]");
            markup = new Markup(userInput);
            explorerUi.MarkAsDirty();
        }

        public Measurement Measure(RenderOptions options, int maxWidth) =>
            ((IRenderable)markup).Measure(options, maxWidth);

        public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) =>
            ((IRenderable)markup).Render(options, maxWidth);
    }
}
