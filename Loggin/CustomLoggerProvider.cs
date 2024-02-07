using Microsoft.Extensions.Logging;

namespace console_explorer.Loggin
{
    [ProviderAlias("CustomLogger")]
    internal class CustomLoggerProvider(IExplorerUi explorerUi) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new CustomLogger(explorerUi);
        }

        public void Dispose()
        {
        }
    }
}
