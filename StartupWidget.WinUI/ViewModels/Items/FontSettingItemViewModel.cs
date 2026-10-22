namespace StartupWidget.ViewModels.Items
{
    internal class FontSettingItemViewModel: SettingItemViewModel
    {
        public double Size { get; set; }
        public IEnumerable<string>? FontFamilies { get; set; }
        public FontSettingItemViewModel(string id, string name, Type dataType, object value, IEnumerable<string> fontFamily, double size) : base(id,name, dataType, value)
        {
            FontFamilies = fontFamily;
            Size = size;
        }
    }
}
