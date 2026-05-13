using System;
using GTA;
using GTA.Native;
using NativeUI;
using MMI_SP.Insurance;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Menu
    {
        internal static UIMenuItem _itemInsure;

        internal static void InsureButtonBuild(UIMenu parentMenu)
        {
            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists())
            {
                RemoveExistingItem(parentMenu);
                return;
            }

            // Obtener estado del botón según el vehículo
            DetermineInsureButtonState(out string description, out bool enabled, out string rightLabel);

            RemoveExistingItem(parentMenu);

            _itemInsure = new UIMenuItem("Asegurar vehículo", description);
            _itemInsure.Enabled = enabled;
            if (enabled)
                _itemInsure.SetRightLabel(rightLabel);

            _itemInsure.Activated += Buttons.Insure;
            parentMenu.AddItem(_itemInsure);
        }

        /// <summary>
        /// Actualiza el estado, descripción y precio del botón "Asegurar vehículo" sin recrearlo.
        /// Se debe llamar cuando cambia el estado del seguro del vehículo (ej. tras cancelar una póliza).
        /// </summary>
        internal static void UpdateInsureButton(UIMenu parentMenu)
        {
            // Buscar el ítem (usamos la referencia estática o búsqueda por texto)
            UIMenuItem insureItem = _itemInsure;
            if (insureItem == null)
            {
                foreach (var item in parentMenu.MenuItems)
                {
                    if (item.Text == "Asegurar vehículo")
                    {
                        insureItem = item;
                        break;
                    }
                }
            }

            if (insureItem == null) return;

            // Obtener el nuevo estado según el vehículo actual
            DetermineInsureButtonState(out string description, out bool enabled, out string rightLabel);

            // Aplicar al ítem existente (sin tocar la colección)
            insureItem.Enabled = enabled;
            insureItem.Description = description;
            insureItem.SetRightLabel(rightLabel);
        }

        /// <summary>
        /// Calcula la descripción, habilitación y etiqueta de precio para el botón de asegurar.
        /// </summary>
        private static void DetermineInsureButtonState(out string description, out bool enabled, out string rightLabel)
        {
            Vehicle veh = Game.Player.LastVehicle;
            if (veh == null || !veh.Exists())
            {
                description = "No hay vehículo disponible";
                enabled = false;
                rightLabel = "";
                return;
            }

            int cost = Core.GetCost(veh);
            string vehName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            bool isInsured = Core.IsInsured(veh);
            bool isInsurable = Core.IsInsurable(veh);

            if (isInsured)
            {
                description = $"Este vehículo ya está asegurado\n{vehName}";
                enabled = false;
                rightLabel = "";
            }
            else if (!isInsurable)
            {
                description = $"No se puede asegurar este vehículo\n{vehName}";
                enabled = false;
                rightLabel = "";
            }
            else
            {
                description = $"Asegurar el último vehículo en uso\n{vehName}";
                enabled = true;
                rightLabel = cost + "$";
            }
        }

        private static void RemoveExistingItem(UIMenu parentMenu)
        {
            if (_itemInsure == null) return;

            _itemInsure.Activated -= Buttons.Insure;
            parentMenu.MenuItems.Remove(_itemInsure);
            _itemInsure = null;
        }
    }
}