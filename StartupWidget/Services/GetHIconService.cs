using StartupWidget.Services.Win32Helpers;

namespace StartupWidget.Services
{
    public class GetHIconService
    {
        private IntPtr hInstance;
        private int resourceid;

        public GetHIconService(IntPtr hinstance,int resourceid)
        {
            this.hInstance = hinstance;
            this.resourceid = resourceid;
        }
        public IntPtr GetHIcon()
        {
            return IconHelper.LoadIconId(hInstance, resourceid);
        }
    }
}