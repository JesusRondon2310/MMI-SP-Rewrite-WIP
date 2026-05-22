using System;
using System.IO;
using GTA.Native;
using NativeUI;

namespace MMI_SP.iFruit
{
    internal class Menu
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private readonly MenuPool _pool;
        private readonly UIMenu _mainMenu;
        private UIMenu _recoverSubmenu;
        internal bool IsVisible => _mainMenu.Visible;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal Menu()
        {
            _pool = new MenuPool();
            _mainMenu = new UIMenu("", "");
            if (File.Exists(Config.BannerImage))
                _mainMenu.SetBannerType(Config.BannerImage);
            _pool.Add(_mainMenu);
        }

        internal void MenuPoolProcessMenus() => _pool.ProcessMenus();

        internal void Show()
        {
            _mainMenu.Visible = true;
            Function.Call(Hash.SET_CURSOR_POSITION, 0.5f, 0.5f);
        }

        internal void Reset(bool rebuild)
        {
            _mainMenu.Clear();
            if (_recoverSubmenu != null) { _recoverSubmenu.Clear(); _recoverSubmenu = null; }
            if (rebuild) Build();
        }

        internal Action OnMainMenuClosed(Action onClosed)
        {
            void handler(UIMenu sender) { if (sender == _mainMenu) onClosed(); }
            _mainMenu.OnMenuClose += handler;
            return () => _mainMenu.OnMenuClose -= handler;
        }

        private void Build()
        {
            BuildRecover();
            _pool.RefreshIndex();
        }

        private void BuildRecover()
        {
            var parentItem = new UIMenuItem("Reclamar vehículo destruido", "Recupera un vehículo destruido");
            _mainMenu.AddItem(parentItem);
            _recoverSubmenu = new UIMenu("", "Selecciona vehículo a recuperar");
            if (File.Exists(Config.BannerImage)) _recoverSubmenu.SetBannerType(Config.BannerImage);
            _pool.Add(_recoverSubmenu);
            _mainMenu.BindMenuToItem(_recoverSubmenu, parentItem);
            MenuBuilder.FillRecover(_recoverSubmenu, () => MenuBuilder.FillRecover(_recoverSubmenu, null));
        }
    }
}