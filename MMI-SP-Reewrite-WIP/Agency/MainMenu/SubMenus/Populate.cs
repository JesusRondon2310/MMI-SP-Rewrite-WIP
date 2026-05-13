using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using NativeUI;
using MMI_SP.Insurance;

namespace MMI_SP.Agency.MainMenu.SubMenus
{
    internal static class Populate
    {
        /// <summary>
        /// Llena un submenú con los vehículos asegurados.
        /// </summary>
        /// <param name="submenu">Submenú a poblar.</param>
        /// <param name="onVehicleSelected">Acción a ejecutar cuando un vehículo es seleccionado, recibe el ID del vehículo.</param>
        /// <param name="emptyMessage">Mensaje a mostrar si la lista está vacía.</param>
        public static void Fill(UIMenu submenu, Action<string> onVehicleSelected, string emptyMessage)
        {
            if (submenu == null) return;
            submenu.Clear();

            List<string> insuredIds = Core.GetInsuredList();
            if (insuredIds.Count == 0)
            {
                AddEmptyItem(submenu, emptyMessage);
                return;
            }

            foreach (string vehId in insuredIds)
            {
                string modelName = GetModelNameFromId(vehId);
                string plate = GetPlateFromId(vehId);

                UIMenuItem item = new UIMenuItem(modelName, $"Matrícula: {plate}");
                item.Activated += (sender, selectedItem) => onVehicleSelected(vehId);
                submenu.AddItem(item);
            }
        }

        private static void AddEmptyItem(UIMenu submenu, string description)
        {
            UIMenuItem emptyItem = new UIMenuItem("Vacío", description);
            emptyItem.Enabled = false;
            submenu.AddItem(emptyItem);
        }

        private static string GetModelNameFromId(string vehicleId)
        {
            string[] parts = vehicleId.Split('_');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int modelHash))
            {
                return Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, modelHash);
            }
            return "Desconocido";
        }

        private static string GetPlateFromId(string vehicleId)
        {
            string[] parts = vehicleId.Split('_');
            return parts.Length >= 2 ? parts[1] : "";
        }
    }
}