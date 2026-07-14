using StartupWidget.Models.Types;
using StartupWidget.Services.Win32Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;

namespace StartupWidget.Services
{
    public class ContextMenuService
    {
        private IntPtr hWnd;
        private IntPtr hMenu;
        private List<IntPtr>? hbitmaps;
        public ContextMenuService(IntPtr hwnd)
        {
            hWnd = hwnd;
            hMenu = IntPtr.Zero;
        }

        public void ShowContextMenu(ICollection<ContextMenuItemBase> MenuItems)
        {
            hbitmaps = new List<nint>();
            hMenu = ContextMenuHelper.CreateMenu();
            for (uint i = 0; i < MenuItems.Count; i++)
            {
                AppendMenu(MenuItems.ElementAt((int)i), i+1);
            }

            Win32Helpers.Point cursorPos;
            ContextMenuHelper.GetCursorPos(out cursorPos);

            int result = ContextMenuHelper.ShowPopupMenuWithIndex(hWnd, hMenu, cursorPos.x, cursorPos.y);

            result--;
            if(result >= 0)
            {
                ((ContextMenuItem)MenuItems.ElementAt(result)).MenuClick();
            }

            ContextMenuHelper.TryDestroyMenu(hMenu);
            hMenu = IntPtr.Zero;
            foreach (var i in hbitmaps) {
                HBitmapHelper.DeleteObject(i);
            }
            hbitmaps = null;
        }
        private void AppendMenu(ContextMenuItemBase trayItem, uint index)
        {
            if (trayItem is ContextMenuItem headerMenu)
            {
                if (headerMenu.Icon != null)
                {
                    IntPtr hbitmap = HBitmapHelper.ConvertToHBitmap((BitmapSource)headerMenu.Icon);;
                    hbitmaps!.Add(hbitmap);
                    ContextMenuHelper.AddMenuItemImage(hMenu, index, headerMenu.Header, hbitmap);
                }
                else
                {
                    ContextMenuHelper.AddMenuItem(hMenu, index, headerMenu.Header);
                }
            }
            if (trayItem is ContextMenuCheckableItem checkable)
            {
                ContextMenuHelper.ChangeMenuCheckState(hMenu, index, checkable.IsChecked);
            }
            if (trayItem is ContextMenuSeparator separator)
            {
                ContextMenuHelper.AddMenuSeparator(hMenu);
            }
        }

        private IntPtr AddIcon(ContextMenuItem trayItem, uint index) 
        {
            if (trayItem.Icon == null) { return IntPtr.Zero; }
            IntPtr hbitmap= HBitmapHelper.ConvertToHBitmap((BitmapSource)trayItem.Icon);
            Debug.WriteLine($"ConvertToHBitmap: {Marshal.GetLastPInvokeError()}");

            ContextMenuHelper.AddMenuItemImage(hMenu, index, trayItem.Header, hbitmap);

            Debug.WriteLine($"InsertMenuError: {Marshal.GetLastPInvokeError()}");
            return hbitmap;
        }
    }
}
