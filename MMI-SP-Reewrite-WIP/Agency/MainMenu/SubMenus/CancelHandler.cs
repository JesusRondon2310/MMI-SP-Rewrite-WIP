using System;
using MMI_SP.Helpers;
using MMI_SP.Insurance;
using NativeUI;
using MMI_SP.Agency.MainMenu.Buttons;
using MMI_SP.PatternMatching;

namespace MMI_SP.Agency.MainMenu.SubMenus
{
    internal class CancelHandler
    {
        public static CancelHandler Instance { get; private set; }

        private UIMenu _submenu;
        private readonly UIMenu _parentMenu;
        private readonly MenuPool _pool;
        private readonly System.Action _onCancelAction;
        public MenuPool Pool => _pool;
        public UIMenu Submenu => _submenu;

        public CancelHandler(UIMenu parentMenu, MenuPool pool, System.Action onCancelAction)
        {
            Instance = this;
            _parentMenu = parentMenu;
            _pool = pool;
            _onCancelAction = onCancelAction;
        }

        public void Build()
        {
            _submenu = Buttons.Build.SubMenu(
                _parentMenu,
                _pool,
                "Cancelar seguro",
                "Elimina un vehículo de tu póliza",
                "Selecciona el vehículo a cancelar",
                OnCancelActivated,
                "No tienes vehículos asegurados.");
        }

        private void OnCancelActivated(string vehicleId)
        {
            var result = Core.Cancel(vehicleId);

            result.Match<bool>(
                onOk: _ =>
                {
                    Notification.Show("CHAR_CARSITE", "Mors Mutual", "Seguro cancelado correctamente.", "");
                    Insure.Update(_parentMenu);
                    ExecuteRebuild.SubMenu(() => Fill.SubMenu(_submenu, OnCancelActivated, "No tienes vehículos asegurados."), _pool);
                    _submenu.Visible = true;
                    _parentMenu.Visible = false;
                    return true;
                },
                onErr: error =>
                {
                    Notification.Show("CHAR_CARSITE", "Mors Mutual", error, "");
                    return false;
                }
            );
        }

        public void Repopulate()
        {
            ExecuteRebuild.SubMenu(() => Fill.SubMenu(_submenu, OnCancelActivated, "No tienes vehículos asegurados."), _pool);
            _onCancelAction?.Invoke();
        }
    }
}