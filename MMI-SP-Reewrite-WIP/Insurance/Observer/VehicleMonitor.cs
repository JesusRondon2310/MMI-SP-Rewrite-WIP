using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class VehicleMonitor
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void CheckForInsuredVehicles(Policies.Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            if (insurer == null) return;

            Vehicle[] allVehicles = World.GetAllVehicles();

            foreach (Vehicle veh in allVehicles)
            {
                if (veh.IsDead) continue;
                if (insuredVehList.Contains(veh)) continue;

                // 1. Comprobar si está asegurado CON la placa original
                bool isInsured = insurer.IsInsured(veh);

                // 2. Si no está asegurado Y tiene placa genérica, cambiarla
                if (!isInsured && veh.Mods.LicensePlate == "46EEK572") veh.Mods.LicensePlate = VehicleIdentifier.GetRandomNumberPlate();

                // 3. Si está asegurado, añadirlo a la lista de vigilancia
                if (isInsured)
                {
                    insuredVehList.Add(veh);

                    // Crear el blip del vehículo asegurado
                    string vehId = VehicleIdentifier.Get(veh);
                    if (!blipsToRemove.ContainsKey(vehId))
                    {
                        var result = BlipManager.AddVehicleBlip(veh);
                        if (result is Ok<Blip> ok)
                        {
                            blipsToRemove[vehId] = ok.Value;
                        }
                    }
                }
            }
        }

        internal static void UpdateInsurance(Policies.Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            for (int i = insuredVehList.Count - 1; i >= 0; i--)
            {
                Vehicle currentVeh = insuredVehList[i];
                if (!currentVeh.Exists())
                {
                    insuredVehList.RemoveAt(i);
                    continue;
                }

                if (currentVeh.IsDead)
                    HandleDestroyedVehicle(insurer, insuredVehList, blipsToRemove, i, currentVeh);
                else
                    HandleAliveVehicle(insurer, currentVeh);
            }
        }

        private static void HandleDestroyedVehicle(Policies.Insurer insurer, List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove, int index, Vehicle currentVeh)
        {
            string vehId = VehicleIdentifier.Get(currentVeh);

            Notification.Show("CHAR_MP_MORS_MUTUAL", "MORS MUTUAL INSURANCE",
                "Tu vehículo ha sido destruido. Llama a Mors Mutual para recuperarlo.", "");

            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Text_Arrive_Tone", "Phone_SoundSet", 1);

            insurer.MarkAsDestroyed(vehId);
            Logger.Debug($"Vehículo marcado como destruido: {vehId}");
            insurer.UpdateVehicleData(currentVeh);

            currentVeh.IsPersistent = false;
            insuredVehList.RemoveAt(index);
            BlipManager.RemoveRecoverBlip(currentVeh, blipsToRemove);
        }

        private static void HandleAliveVehicle(Policies.Insurer insurer, Vehicle currentVeh)
        {
            if (Game.Player.Character.CurrentVehicle == currentVeh && GameplayCamera.IsRendering)
                insurer.UpdateVehicleData(currentVeh);

            PersistenceManager.SetPersistence(currentVeh, true);
        }
    }
}