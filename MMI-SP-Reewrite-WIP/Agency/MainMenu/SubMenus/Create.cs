using NativeUI;

namespace MMI_SP.Agency.MainMenu.SubMenus
{
    internal static class Create
    {
        public static UIMenu AddSubMenu(MenuPool pool, UIMenu parent,
             string title, string subtitle, string itemText, string itemDescription)
        {
            UIMenu submenu = new UIMenu(title, subtitle);
            pool.Add(submenu);

            UIMenuItem item = new UIMenuItem(itemText, itemDescription);
            parent.AddItem(item);

            parent.BindMenuToItem(submenu, item);
            return submenu;
        }
    }
}