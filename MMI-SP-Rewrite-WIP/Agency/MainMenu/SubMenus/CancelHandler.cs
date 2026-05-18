using System;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
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
            var submenuResult = Buttons.Build.SubMenu(
                _parentMenu,
                _pool,
                "Cancelar seguro",
                "Elimina un vehículo de tu póliza",
                "Selecciona el vehículo a cancelar",
                OnCancelActivated,
                "No tienes vehículos asegurados.");

            if (submenuResult is Ok<UIMenu> ok)
                _submenu = ok.Value;
            else
                Logger.Error($"Error al crear submenú Cancelar: {((Err<UIMenu>)submenuResult).Message}");
        }

        private void OnCancelActivated(string vehicleId)
        {
            var result = Manager.Cancel(vehicleId);

            result.match<bool>(
                onOk: _ =>
                {
                    Notification.Show("CHAR_CARSITE", "Mors Mutual", "Seguro cancelado correctamente.", "");
                    Insure.Update(_parentMenu);
                    ExecuteRebuild.SubMenu(() =>
                    {
                        var fillResult = Fill.SubMenu(_submenu, OnCancelActivated, "No tienes vehículos asegurados.");
                        if (fillResult is Err<bool> err)
                            Logger.Error($"Error al refrescar submenú Cancelar: {err.Message}");
                    }, _pool);
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
            ExecuteRebuild.SubMenu(() =>
            {
                var fillResult = Fill.SubMenu(_submenu, OnCancelActivated, "No tienes vehículos asegurados.");
                if (fillResult is Err<bool> err)
                    Logger.Error($"Error al repoblar submenú Cancelar: {err.Message}");
            }, _pool);
            _onCancelAction?.Invoke();
        }
    }
}