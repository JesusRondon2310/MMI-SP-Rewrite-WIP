using System;
using NativeUI;

namespace MMI_SP.Agency.Menu
{
    internal class UI
    {
        private readonly MenuPool _pool;
        private readonly UIMenu _mainMenu;

        internal UI()
        {
            (_pool, _mainMenu) = Interface.Build();  // Ya incluye el banner
        }

        internal void Show() => _mainMenu.Visible = true;
        internal void Hide() => _mainMenu.Visible = false;
        internal void Update() => _pool.ProcessMenus();
        internal void Reset() => Execute.Rebuild(_mainMenu);

        internal void Closed(Action callback)
        {
            MenuCloseEvent handler = (sender) => callback();
            _mainMenu.OnMenuClose += handler;
        }
    }
}