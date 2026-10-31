using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Windows.System;

namespace StartupWidget.Models.Types
{
    public class ShortcutKeys
    {
        public ShortcutKeys()
        {
            Modifires = KeyModifiers.None;
            Keys = VirtualKey.None;
        }
        public ShortcutKeys(KeyModifiers modifires, VirtualKey key)
        {
            Modifires = modifires;
            Keys = key;
        }
        public ShortcutKeys(ShortcutKeys shortcut)
        {
            Modifires = shortcut.Modifires;
            Keys = shortcut.Keys;
        }


        public KeyModifiers Modifires;
        public VirtualKey Keys;
        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Modifires != KeyModifiers.None)
            {
                sb.Append(string.Format("{0},", Modifires.ToString()));
            }
            if (Keys != VirtualKey.None)
            {
                sb.Append(string.Format("{0}", Keys.ToString()));
            }
            return sb.ToString();
        }

        public static ShortcutKeys Parse(string value)
        {
            value = value.Trim();
            value = value.Replace(" ", "");
            ShortcutKeys sckeys = new ShortcutKeys(KeyModifiers.None, VirtualKey.None);
            foreach(var i in value.Split(','))
            {
                switch (i)
                {
                    case nameof(KeyModifiers.Alt):
                        sckeys.Modifires |= KeyModifiers.Alt; break;
                    case nameof(KeyModifiers.Control):
                        sckeys.Modifires |= KeyModifiers.Control; break;
                    case nameof(KeyModifiers.Shift):
                        sckeys.Modifires |= KeyModifiers.Shift; break;
                    case nameof(KeyModifiers.Win):
                        sckeys.Modifires |= KeyModifiers.Win; break;
                    default:
                        if (!VirtualKey.TryParse(i.ToUpper(), out sckeys.Keys))
                        {
                            sckeys.Keys = VirtualKey.None;
                        }
                        break;
                }
            }
            return sckeys;
        }
    }
    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        // Either WINDOWS key was held down. These keys are labeled with the Windows logo.
        // Keyboard shortcuts that involve the WINDOWS key are reserved for use by the
        // operating system.
        Win = 8,
    }
}
