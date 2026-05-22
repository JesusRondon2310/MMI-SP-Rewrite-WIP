using System.Collections.Generic;
using GTA;
using GTA.Native;
using NativeUI;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;

namespace MMI_SP.iFruit
{
    internal static class MenuBuilder
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void FillRecover(UIMenu submenu, System.Action onRefresh)
        {
            submenu.Clear();
            List<string> ids = Manager.GetDestroyedList();

            if (ids.Count == 0)
            {
                var empty = new UIMenuItem("Vacío", "No tienes vehículos destruidos.");
                empty.Enabled = false;
                submenu.AddItem(empty);
                return;
            }

            foreach (string id in ids)
            {
                string label = GetVehicleLabel(id);
                int cost = Calculator.GetRecoverCost(id);
                var item = new UIMenuItem($"{label} ({cost}$)", "Coste de recuperación");
                submenu.AddItem(item);
                string capturedId = id;
                int capturedCost = cost;
                submenu.OnItemSelect += (sender, selected, index) =>
                {
                    if (selected != item) return;
                    if (Game.Player.Money < capturedCost)
                    {
                        Notification.Show("Mors Mutual Insurance", "No tienes suficiente dinero.", "");
                        MMISound.Play(MMISound.SoundFamily.NoMoney);
                        return;
                    }
                    Game.Player.Money -= capturedCost;
                    Manager.RecoverVehicle(capturedId).match<bool>(
                        onOk: _ =>
                        {
                            Notification.Show("Mors Mutual Insurance", "Reclamación aprobada.", "Tu vehículo está en el depósito.");
                            MMISound.Play(MMISound.SoundFamily.Okay);
                            onRefresh?.Invoke();
                            return true;
                        },
                        onErr: error =>
                        {
                            Game.Player.Money += capturedCost;
                            Notification.Show("Mors Mutual Insurance", error, "");
                            return false;
                        }
                    );
                };
            }
        }

        private static string GetVehicleLabel(string vehicleId)
        {
            string[] parts = vehicleId.Split('_');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int hash))
            {
                string name = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, hash);
                if (!string.IsNullOrEmpty(name)) return name;
            }
            return vehicleId;
        }
    }
}