namespace StartupWidget.Models.Types
{
    public delegate void MenuClickEventHandler(object sender, EventArgs e);
    public class TaskTrayItem : TaskTrayMenuItemBase
    {
        public string Header { get; set; }
        public event MenuClickEventHandler? MenuClicked;

        public TaskTrayItem(string header)
        {
            Header = header;
        }

        public virtual void MenuClick()
        {
            MenuClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}