using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartupWidget.Services
{
    static class StartupService
    {
        const string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
        public static bool SetStartup(bool startup)
        {
            using(RegistryKey? rKey = Registry.CurrentUser.OpenSubKey(keyName, true))
            {
                if (rKey == null) return false;
                if (startup)
                {
                    rKey.SetValue(Application.ProductName, Application.ExecutablePath);
                }
                else
                {
                    rKey.DeleteValue(Application.ProductName, false);
                }
                rKey.Close();
            }
            return true;
        }

        public static bool IsStartupEnabled()
        {
            using (RegistryKey? rKey=Registry.CurrentUser.OpenSubKey(keyName))
            {
                if (rKey == null) return false;
                return (rKey.GetValue(Application.ProductName) != null) ? true : false;
            }
        }
    }
}
