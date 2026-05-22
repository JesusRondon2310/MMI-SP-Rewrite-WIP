using GTA;
using GTA.Native;
using MMI_SP.Debug;
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

                bool isInsured = insurer.IsInsured(veh);

                if (!isInsured && veh.Mods.LicensePlate == "46EEK572") veh.Mods.LicensePlate = VehicleIdentifier.GetRandomNumberPlate();

                if (isInsured)
                {
                    insuredVehList.Add(veh);

                    string vehId = VehicleIdentifier.Get(veh);
                    if (!blipsToRemove.ContainsKey(vehId))
                    {
                        BlipManager.AddVehicleBlip(veh).match<bool>(
                            onOk: blip =>
                            {
                                blipsToRemove[vehId] = blip;
                                return true;
                            },
                            onErr: error =>
                            {
                                Logger.Error($"Error al crear blip del vehículo: {error}");
                                return false;
                            }
                        );
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

            Notification.Show("Mors Mutual Insurance", "Vehículo Destruido.", "Llama a Mors Mutual o ve a nuestras oficinas para recuperarlo.");

            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Text_Arrive_Tone", "Phone_SoundSet", 1);

            insurer.MarkAsDestroyed(vehId).match<bool>(
                onOk: _ => true,
                onErr: error =>
                {
                    Logger.Error($"Error al marcar vehículo como destruido: {error}");
                    return false;
                }
            );

            insurer.UpdateVehicleData(currentVeh).match<bool>(
                onOk: _ => true,
                onErr: error =>
                {
                    Logger.Error($"Error al actualizar datos del vehículo: {error}");
                    return false;
                }
            );

            currentVeh.IsPersistent = false;
            insuredVehList.RemoveAt(index);
            BlipManager.RemoveRecoverBlip(currentVeh, blipsToRemove);
        }

        private static void HandleAliveVehicle(Policies.Insurer insurer, Vehicle currentVeh)
        {
            if (Game.Player.Character.CurrentVehicle == currentVeh && GameplayCamera.IsRendering)
                insurer.UpdateVehicleData(currentVeh).match<bool>(
                    onOk: _ => true,
                    onErr: error =>
                    {
                        Logger.Error($"Error al actualizar datos del vehículo en movimiento: {error}");
                        return false;
                    }
                );

            PersistenceManager.SetPersistence(currentVeh, true);
        }
    }
}