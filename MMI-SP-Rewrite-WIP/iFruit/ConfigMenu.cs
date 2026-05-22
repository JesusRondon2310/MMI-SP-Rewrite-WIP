using NativeUI;
using GTA.Native;
using GTA;

namespace MMI_SP.iFruit
{
    internal class ConfigMenu
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private readonly MenuPool _pool;
        private readonly UIMenu _mainMenu;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal ConfigMenu()
        {
            _pool = new MenuPool();
            _mainMenu = new UIMenu("Configuración MMI", "Ajustes del mod");
            if (System.IO.File.Exists(Config.BannerImage))
                _mainMenu.SetBannerType(Config.BannerImage);
            _pool.Add(_mainMenu);

            BuildGeneral();
            BuildiFruit();
            BuildInsurance();
            BuildBringVehicle();
            _pool.RefreshIndex();
        }

        internal void Show()
        {
            _mainMenu.Visible = true;
            Function.Call(Hash.SET_CURSOR_POSITION, 0.5f, 0.5f);
        }

        internal void MenuPoolProcessMenus() => _pool.ProcessMenus();

        private void BuildGeneral()
        {
            UIMenu sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "General");
            ConfigMenuBuilder.AddCheckbox(sub, "General", "PersistentInsuredVehicles",
                Config.PersistentVehicles, "Vehículos persistentes");
        }

        private void BuildiFruit()
        {
            UIMenu sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "iFruit");
            ConfigMenuBuilder.AddListInt(sub, "iFruit", "PhoneVolume",
                Config.iFruitVolume, "Volumen del teléfono", 0, 100, 5);
        }

        private void BuildInsurance()
        {
            UIMenu sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "Seguros");
            ConfigMenuBuilder.AddListFloat(sub, "Insurance", "InsuranceCostMultiplier",
                Config.InsuranceMult, "Multiplicador seguro", 0f, 10f, 0.1f);
            ConfigMenuBuilder.AddListFloat(sub, "Insurance", "RecoverCostMultiplier",
                Config.RecoverMult, "Multiplicador recuperación", 0f, 10f, 0.1f);
        }

        private void BuildBringVehicle()
        {
            UIMenu sub = ConfigMenuBuilder.AddSubMenu(_pool, _mainMenu, "Traer vehículo");
            ConfigMenuBuilder.AddListInt(sub, "BringVehicle", "BringVehicleBasePrice",
                Config.BringVehicleBasePrice, "Precio base", 0, 2000, 50);
            ConfigMenuBuilder.AddCheckbox(sub, "BringVehicle", "BringVehicleInstant",
                Config.BringVehicleInstant, "Entrega instantánea");
            ConfigMenuBuilder.AddListInt(sub, "BringVehicle", "BringVehicleRadius",
                Config.BringVehicleRadius, "Radio de búsqueda", 10, 2000, 5);
            ConfigMenuBuilder.AddListInt(sub, "BringVehicle", "BringVehicleTimeout",
                Config.BringVehicleTimeout, "Tiempo de espera", 1, 30, 1);
        }
    }
}