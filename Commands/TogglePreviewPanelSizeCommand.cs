namespace console_explorer.Commands
{
    public class TogglePreviewPanelSizeCommand : IUndoableCommand   
    {
        private readonly IExplorerUi explorerUi;
        public string Name => "Toggle preview panel size";

        public TogglePreviewPanelSizeCommand(IExplorerUi ui)
        {
            explorerUi = ui;
        }

        public Task ExecuteAsync()
        {
            if(explorerUi.RightRatio == 3)
            {
                explorerUi.UpdateRightRatio(2);
            }
            else if(explorerUi.RightRatio == 2)
            {
                explorerUi.UpdateRightRatio(1);
            }
            else if(explorerUi.RightRatio == 1)
            {
                explorerUi.UpdateRightRatio(0);
            }
            else if(explorerUi.RightRatio == 0)
            {
                explorerUi.UpdateRightRatio(3);
            }
            return Task.CompletedTask;
        }

        public Task UndoAsync()
        {
            if(explorerUi.RightRatio == 3)
            {
                explorerUi.UpdateRightRatio(0);
            }
            else if(explorerUi.RightRatio == 2)
            {
                explorerUi.UpdateRightRatio(3);
            }
            else if(explorerUi.RightRatio == 1)
            {
                explorerUi.UpdateRightRatio(2);
            }
            else if(explorerUi.RightRatio == 0)
            {
                explorerUi.UpdateRightRatio(1);
            }
            return Task.CompletedTask;
        }
    }
}
