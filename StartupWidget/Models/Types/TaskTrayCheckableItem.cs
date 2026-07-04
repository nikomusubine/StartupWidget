namespace StartupWidget.Models.Types
{
    public class TaskTrayCheckableItem:TaskTrayItem
    {
        public MenuClickEventHandler CheckableValueChanged;
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                ValueChanged();
            }
        }

        private void ValueChanged()
        {
            if (CheckableValueChanged != null)
            {
                CheckableValueChanged(this, new EventArgs());
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
