using System;
using System.Collections.Generic;
using NativeUI;

namespace MMI_SP.iFruit
{
    internal static class ConfigMenuBuilder
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void AddCheckbox(UIMenu menu, string section, string key, bool value, string label)
        {
            var item = new UIMenuCheckboxItem(label, value, "");
            menu.AddItem(item);
            menu.OnCheckboxChange += (sender, changed, index) =>
            {
                if (changed != item) return;
                Config.SetSetting(section, key, changed.Checked.ToString());
                Config.UpdateValue(key, changed.Checked);
                Config.SaveSettings();
            };
        }

        internal static void AddListInt(UIMenu menu, string section, string key, int value, string label, int start, int stop, int step)
        {
            var values = new List<dynamic>();
            int counter = 0;
            bool found = false;
            for (int i = start; i <= stop; i += step)
            {
                values.Add(i);
                if (!found)
                {
                    if (value == i) found = true;
                    else counter++;
                }
            }
            var item = new UIMenuListItem(label, values, counter, "");
            menu.AddItem(item);
            menu.OnListChange += (sender, changed, index) =>
            {
                if (changed != item) return;
                int val = (int)changed.Items[index];
                Config.SetSetting(section, key, val.ToString());
                Config.UpdateValue(key, val);
                Config.SaveSettings();
            };
        }

        internal static void AddListFloat(UIMenu menu, string section, string key, float value, string label, float start, float stop, float step)
        {
            var values = new List<dynamic>();
            int counter = 0;
            bool found = false;
            for (float i = start; i <= stop; i += step)
            {
                float rounded = (float)Math.Round(i, 1, MidpointRounding.AwayFromZero);
                values.Add(rounded);
                if (!found)
                {
                    if (Math.Abs(value - rounded) < 0.001f) found = true;
                    else counter++;
                }
            }
            var item = new UIMenuListItem(label, values, counter, "");
            menu.AddItem(item);
            menu.OnListChange += (sender, changed, index) =>
            {
                if (changed != item) return;
                float val = (float)changed.Items[index];
                Config.SetSetting(section, key, val.ToString().Replace(",", "."));
                Config.UpdateValue(key, val);
                Config.SaveSettings();
            };
        }

        internal static UIMenu AddSubMenu(MenuPool pool, UIMenu parent, string label)
        {
            var item = new UIMenuItem(label);
            parent.AddItem(item);
            var sub = new UIMenu("Configuración MMI", label);
            pool.Add(sub);
            parent.BindMenuToItem(sub, item);
            if (System.IO.File.Exists(Config.BannerImage))
                sub.SetBannerType(Config.BannerImage);
            return sub;
        }
    }
}