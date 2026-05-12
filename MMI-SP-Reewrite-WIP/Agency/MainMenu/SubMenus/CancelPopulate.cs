using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using NativeUI;

namespace MMI_SP.Agency.MainMenu.SubMenus
{
    internal static class CancelPopulate
    {
        /// <summary>
        /// Llena el submenú con los vehículos asegurados.
        /// Recibe una acción que se ejecutará al seleccionar un vehículo, pasando su ID.
        /// </summary>
        public static void Fill(UIMenu submenu, List<string> insuredVehicleIds,
            Action<string> onVehicleSelected)
        {
            foreach (string vehId in insuredVehicleIds)
            {
                string modelName = GetModelNameFromId(vehId);
                string plate = GetPlateFromId(vehId);

                UIMenuItem item = new UIMenuItem(modelName, $"Matrícula: {plate}");
                item.Activated += (sender, selectedItem) => onVehicleSelected(vehId);
                submenu.AddItem(item);
            }
        }

        /// <summary>
        /// Añade un ítem no seleccionable con el mensaje indicado.
        /// </summary>
        public static void AddEmptyItem(UIMenu submenu, string description)
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