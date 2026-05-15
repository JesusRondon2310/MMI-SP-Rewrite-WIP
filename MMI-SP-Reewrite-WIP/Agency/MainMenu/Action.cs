using GTA;
using NativeUI;
using MMI_SP.Helpers;
using MMI_SP.Insurance;
using MMI_SP.PatternMatching;

namespace MMI_SP.Agency.MainMenu
{
    internal static class Action
    {
        private const string NotifyChar = "CHAR_CARSITE";
        private const string NotifyTitle = "Mors Mutual";

        internal static void OnActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (selectedItem == null || !selectedItem.Enabled) return;

            Vehicle veh = Game.Player.LastVehicle;
            var result = Core.Insure(veh);

            result.Match<bool>(
                onOk: _ =>
                {
                    Notification.Show(NotifyChar, NotifyTitle, "Vehículo asegurado correctamente.", "");
                    selectedItem.Enabled = false;
                    selectedItem.SetRightLabel("");
                    Cancel.Refresh();
                    return true;
                },
                onErr: error =>
                {
                    Notification.Show(NotifyChar, NotifyTitle, error, "");
                    return false;
                }
            );
        }
    }
}