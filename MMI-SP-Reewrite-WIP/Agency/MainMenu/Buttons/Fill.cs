using System;
using System.Collections.Generic;
using GTA.Native;
using NativeUI;
using MMI_SP.Insurance.Policies;

namespace MMI_SP.Agency.MainMenu.Buttons
{
    internal static class Fill
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void SubMenu(UIMenu submenu, Action<string> onVehicleSelected, string emptyMessage, bool showDestroyed = false)
        {
            if (submenu == null) return;
            submenu.Clear();

            List<string> items = showDestroyed ? Manager.GetDestroyedList() : Manager.GetInsuredList();

            if (items.Count == 0)
            {
                UIMenuItem emptyItem = new UIMenuItem("Vacío", emptyMessage);
                emptyItem.Enabled = false;
                submenu.AddItem(emptyItem);
                return;
            }

            foreach (string vehId in items)
            {
                string modelName = GetModelName(vehId);
                string plate = GetPlate(vehId);
                UIMenuItem item = new UIMenuItem(modelName, $"Matrícula: {plate}");
                item.Activated += (sender, selectedItem) => onVehicleSelected(vehId);
                submenu.AddItem(item);
            }
        }

        private static string GetModelName(string vehicleId)
        {
            string[] parts = vehicleId.Split('_');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int modelHash))
            {
                string name = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, modelHash);
                if (!string.IsNullOrEmpty(name)) return name;
            }
            return "Desconocido";
        }

        private static string GetPlate(string vehicleId)
        {
            string[] parts = vehicleId.Split('_');
            return parts.Length >= 2 ? parts[1] : "";
        }
    }
}