using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

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
                    string? processPath = Environment.ProcessPath;
                    if (string.IsNullOrWhiteSpace(processPath))
                    {
                        return false;
                    }

                    rKey.SetValue(GetProductName(), processPath);
                }
                else
                {
                    rKey.DeleteValue(GetProductName(), false);
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
                return (rKey.GetValue(GetProductName()) != null) ? true : false;
            }
        }

private static string GetProductName()
{
    Assembly asm = Assembly.GetExecutingAssembly();
    AssemblyProductAttribute? prodAttr = asm.GetCustomAttribute<AssemblyProductAttribute>();
    if (!string.IsNullOrWhiteSpace(prodAttr?.Product))
    {
        return prodAttr.Product;
    }

    return asm.GetName().Name ?? "StartupWidget";
}
    }
}
