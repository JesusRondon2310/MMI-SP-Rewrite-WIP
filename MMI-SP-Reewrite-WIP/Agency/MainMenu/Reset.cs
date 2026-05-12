using MMI_SP.Agency.MainMenu.SubMenus;
using NativeUI;

namespace MMI_SP.Agency.MainMenu
{
    internal static class ExecuteRebuild
    {
        internal delegate void PopulateSubMenu();

        internal static void MainMenu(UIMenu mainMenu, MenuPool pool)
        {
            mainMenu.Clear();
            Menu.InsureButtonBuild(mainMenu);
            Cancel.Build(mainMenu, pool);
            pool.RefreshIndex();
        }
        
        internal static void SubMenu(PopulateSubMenu submenu, MenuPool pool)
        {
            submenu();
            pool.RefreshIndex();
        }
    }
}