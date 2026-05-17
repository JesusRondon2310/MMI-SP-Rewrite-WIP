using GTA;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Drawing;

namespace MMI_SP.Insurance.Observer
{
    internal static class VehicleChangeHandler
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static Vehicle _previousVehicle = null;
        private static int _insuranceIconTimer = 0;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void Handle(
            List<Vehicle> insuredVehList,
            List<Vehicle> recoveredVehList,
            Dictionary<string, Blip> blipsToRemove)
        {
            HandleVehicleChange(blipsToRemove);
            DrawInsuranceSprite();
        }

        private static void HandleVehicleChange(Dictionary<string, Blip> blipsToRemove)
        {
            if (_previousVehicle == Game.Player.Character.CurrentVehicle) return;

            Vehicle previous = _previousVehicle;
            _previousVehicle = Game.Player.Character.CurrentVehicle;

            if (_previousVehicle != null)
            {
                BlipManager.RemoveRecoverBlip(_previousVehicle, blipsToRemove);
                _insuranceIconTimer = Game.GameTime + 4270;
            }
            else if (previous != null && previous.Exists())
            {
                RestoreBlip(previous, blipsToRemove);

                if (Insurer.Instance.IsInsured(previous))
                {
                  Insurer.Instance.UpdateVehicleData(previous)
                   .Match<bool>(
                     onOk: _ => true,
                     onErr: error =>
                     {
                       Logger.Error($"Error al guardar posición del vehículo: {error}");
                       return false;
                     }
                   );
                }
            }
        }

        private static void RestoreBlip(Vehicle vehicle, Dictionary<string, Blip> blipsToRemove)
        {
            string vehId = VehicleIdentifier.Get(vehicle);
            if (Insurer.Instance.IsInsured(vehicle) && !blipsToRemove.ContainsKey(vehId))
            {
                var result = BlipManager.AddVehicleBlip(vehicle);
                if (result is Ok<Blip> ok)
                {
                    blipsToRemove[vehId] = ok.Value;
                }
            }
        }

        private static void DrawInsuranceSprite()
        {
            if (Game.GameTime >= _insuranceIconTimer) return;

            Vehicle currentVeh = Game.Player.Character.CurrentVehicle;
            if (currentVeh == null || !Insurer.IsInsurable(currentVeh)) return;

            bool isInsured = Insurer.Instance.IsInsured(currentVeh);
            Color color = isInsured ? Color.FromArgb(35, 199, 128) : Color.FromArgb(190, 0, 50);
            Sprite.InsuranceStatus(Config.InsuranceImage, 1225f, 600f, color);
        }
    }
}