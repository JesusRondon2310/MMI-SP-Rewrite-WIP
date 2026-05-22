using GTA;
using NativeUI;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using MMI_SP.Dialogue;
using MMI_SP.Agency.Office.Ambient;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Action
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private const string NotifyTitle = "Mors Mutual Insurance";

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void OnActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (selectedItem == null || !selectedItem.Enabled) return;

            Vehicle veh = Game.Player.LastVehicle;
            var result = Manager.Insure(veh);

            result.match<bool>(
                onOk: _ =>
                {
                    Notification.Show(NotifyTitle, "Vehículo asegurado correctamente.", "");
                    Core.PlayRandom(Core.SpeechType.OfficeSomething, NpcHandler.CurrentNpc);
                    selectedItem.Enabled = false;
                    selectedItem.SetRightLabel("");
                    Cancel.Refresh();
                    return true;
                },
                onErr: error =>
                {
                    Notification.Show(NotifyTitle, error, "");
                    Core.PlayRandom(Core.SpeechType.OfficeSomething, NpcHandler.CurrentNpc);
                    return false;
                }
            );
        }
    }
}