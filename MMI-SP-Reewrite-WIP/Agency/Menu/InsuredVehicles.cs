using GTA;
using GTA.Native;
using NativeUI;
using System;

namespace MMI_SP.Agency.Menu
{
    internal static class Menu
    {
        private static UIMenuItem _itemInsure;

        public static void InsureButtonBuild(UIMenu parentMenu)
        {
            if (Manager.Instance == null) return;

            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists()) return;

            if (_itemInsure != null)
            {
                _itemInsure.Activated -= Buttons.Insure;
                parentMenu.MenuItems.Remove(_itemInsure);
            }

            int cost = Manager.Instance.GetVehicleInsuranceCost(veh);
            string vehName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            bool isInsured = Manager.Instance.IsVehicleInsured(veh);
            bool isInsurable = Manager.IsVehicleInsurable(veh);

            string title = "Asegurar vehículo";
            string description;
            bool enabled = true;

            if (isInsured)
            {
                description = $"Este vehículo ya está asegurado\n{vehName}";
                enabled = false;
            }
            else if (!isInsurable)
            {
                description = $"No se puede asegurar este vehículo\n{vehName}";
                enabled = false;
            }
            else
            {
                description = $"Coste: {cost}$\n{vehName}";
            }

            _itemInsure = new UIMenuItem(title, description);
            _itemInsure.Enabled = enabled;
            if (enabled)
                _itemInsure.Activated += Buttons.Insure;

            parentMenu.AddItem(_itemInsure);
        }
    }
}