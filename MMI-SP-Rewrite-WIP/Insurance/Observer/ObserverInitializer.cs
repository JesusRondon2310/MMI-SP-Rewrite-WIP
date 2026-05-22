using GTA;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Insurance.Observer
{
    internal static class ObserverInitializer
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void RestoreVehiclesFromDatabase(List<Vehicle> insuredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            List<DB.VehicleData> allVehicles = Insurer.Instance.GetInsuredListFull();
            if (allVehicles == null || allVehicles.Count == 0) return;

            foreach (DB.VehicleData data in allVehicles)
            {
                if (data.IsDestroyed) continue;

                string vehId = data.Id;
                bool alreadyExists = World.GetAllVehicles().Any(v => VehicleIdentifier.Get(v) == vehId);
                if (alreadyExists) continue;

                var spawnResult = DB.VehicleSpawnManager.SpawnVehicle(data);
                if (spawnResult is Err<Vehicle> spawnErr)
                {
                    Logger.Error($"Error al restaurar vehículo {vehId}: {spawnErr.Message}");
                    continue;
                }

                Vehicle spawned = ((Ok<Vehicle>)spawnResult).Value;
                insuredVehList.Add(spawned);

                var blipResult = BlipManager.AddVehicleBlip(spawned);
                if (blipResult is Err<Blip> blipErr)
                    Logger.Error($"Error al crear blip para vehículo restaurado {vehId}: {blipErr.Message}");
                else
                    blipsToRemove[vehId] = ((Ok<Blip>)blipResult).Value;
            }
        }
    }
}