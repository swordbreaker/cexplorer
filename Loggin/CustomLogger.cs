using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace console_explorer.Loggin
{
    public sealed class CustomLogger(IExplorerUi explorerUi) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    explorerUi.UpdateStatus(new Markup($"[grey]{formatter(state, exception).EscapeMarkup()} [/]"));
                    break;
                case LogLevel.Information:
                    explorerUi.UpdateStatus(new Markup($"[white]{formatter(state, exception).EscapeMarkup()} [/]"));
                    break;
                case LogLevel.Warning:
                    explorerUi.UpdateStatus(new Markup($"[yellow]{formatter(state, exception).EscapeMarkup()} [/]"));
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    explorerUi.UpdateStatus(new Markup($"[bold red]{formatter(state, exception).EscapeMarkup()} [/]"));
                    break;
            }

            
        }
    }
}
