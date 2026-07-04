namespace StartupWidget.ViewModels.Items
{
    internal class FontSettingItemViewModel: SettingItemViewModel
    {
        public float Size { get; set; }
        public IEnumerable<string>? FontFamilies { get; set; }
        public FontSettingItemViewModel(string id, string name, Type dataType, object value, IEnumerable<string> fontFamily, float size) : base(id,name, dataType, value)
        {
            Name = name;
            Value = value;
            DataType = dataType;
            FontFamilies = fontFamily;
            Size = size;
        }
    }
}
