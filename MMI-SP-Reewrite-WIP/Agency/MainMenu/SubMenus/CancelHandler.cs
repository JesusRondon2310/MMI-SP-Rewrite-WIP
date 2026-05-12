using MMI_SP.Helpers;
using MMI_SP.Insurance;
using NativeUI;
using System;
using System.Collections.Generic;

namespace MMI_SP.Agency.MainMenu.SubMenus
{
    internal class CancelHandler
    {
        private UIMenu _submenu;
        private readonly UIMenu _parentMenu;
        private readonly MenuPool _pool;
        private readonly Action _onCancelAction;
        public MenuPool Pool => _pool;

        public CancelHandler(UIMenu parentMenu, MenuPool pool, Action onCancelAction)
        {
            _parentMenu = parentMenu;
            _pool = pool;
            _onCancelAction = onCancelAction;
        }

        public void Build()
        {
            _submenu = Create.AddSubMenu(_pool, _parentMenu, "Cancelar seguro",
                "Selecciona el vehículo a cancelar", "Cancelar seguro",
                "Elimina un vehículo de tu póliza");

            if (System.IO.File.Exists(Config.BannerImage))
                _submenu.SetBannerType(Config.BannerImage);

            // Poblar inicialmente
            Populate.Fill(_submenu, OnCancelActivated, "No tienes vehículos asegurados.");
        }

        private void OnCancelActivated(string vehicleId)
        {
            Core.Cancel(vehicleId);
            Notification.Show("CHAR_CARSITE", "Mors Mutual", "Seguro cancelado correctamente.", "");
            Populate.Fill(_submenu, OnCancelActivated, "No tienes vehículos asegurados."); // Refresca el submenú
            _onCancelAction?.Invoke();
        }

        public void Repopulate()
        {
            Populate.Fill(_submenu, OnCancelActivated, "No tienes vehículos asegurados.");
            _pool.RefreshIndex();
            _onCancelAction?.Invoke();
        }
    }
}