namespace StartupWidget.Models.Types
{
    public class TaskTrayCheckableItem:TaskTrayItem
    {
        public event MenuClickEventHandler? CheckableValueChanged;
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value) return;
                _isChecked = value;
                CheckableValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public override void MenuClick()
        {
            IsChecked = !IsChecked;
            base.MenuClick();
        }

        public TaskTrayCheckableItem(string header, bool ischecked) : this(header)
        {
            _isChecked = ischecked;
        }
        public TaskTrayCheckableItem(string header):base(header)
        {
            _isChecked = false;
        }
    }
}
