using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using NativeUI;
using MMI_SP.Agency.MainMenu.Buttons;

namespace MMI_SP.Agency.MainMenu.SubMenus
{
    internal class RecoverHandler
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        public static RecoverHandler Instance { get; private set; }

        private UIMenu _submenu;
        private readonly UIMenu _parentMenu;
        private readonly MenuPool _pool;
        public MenuPool Pool => _pool;
        public UIMenu Submenu => _submenu;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public RecoverHandler(UIMenu parentMenu, MenuPool pool)
        {
            Instance = this;
            _parentMenu = parentMenu;
            _pool = pool;
        }

        public void Build()
        {
            var submenuResult = Buttons.Build.SubMenu(
                _parentMenu,
                _pool,
                "Recuperar vehículo",
                "Vehículos asegurados que han sido destruidos",
                "Selecciona el vehículo a recuperar",
                OnRecoverActivated,
                "No tienes vehículos destruidos.",
                showDestroyed: true);

            if (submenuResult is Ok<UIMenu> ok)
                _submenu = ok.Value;
            else
                Logger.Error($"Error al crear submenú Recuperar: {((Err<UIMenu>)submenuResult).Message}");
        }

        private void OnRecoverActivated(string vehicleId)
        {
            var result = Manager.RecoverVehicle(vehicleId);

            result.Match<bool>(
                onOk: _ =>
                {
                    Notification.Show("CHAR_MP_MORS_MUTUAL", "MORS MUTUAL INSURANCE",
                        "Vehículo recuperado", "Tu vehículo ha sido entregado en el depósito.");
                    Repopulate();
                    _submenu.Visible = true;
                    _parentMenu.Visible = false;
                    return true;
                },
                onErr: error =>
                {
                    Notification.Show("CHAR_MP_MORS_MUTUAL", "MORS MUTUAL INSURANCE", error, "");
                    return false;
                }
            );
        }

        public void Repopulate()
        {
            ExecuteRebuild.SubMenu(() =>
            {
                var fillResult = Fill.SubMenu(_submenu, OnRecoverActivated, "No tienes vehículos destruidos.", showDestroyed: true);
                if (fillResult is Err<bool> err)
                    Logger.Error($"Error al repoblar submenú Recuperar: {err.Message}");
            }, _pool);
        }
    }
}