using NativeUI;

namespace MMI_SP.Agency.Menu
{
    internal static class Execute
    {
        internal static void Rebuild(UIMenu mainMenu)
        {
            mainMenu.Clear();
            Menu.InsureButtonBuild(mainMenu);
        }
    }
}