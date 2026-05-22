using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;

namespace MMI_SP.Insurance.Observer
{
    internal static class PersistenceManager
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void SetPersistence(Vehicle veh, bool persistent)
        {
            if (!Config.PersistentVehicles) return;
            if (!persistent) return;
            Function.Call(Hash.SET_ENTITY_AS_MISSION_ENTITY, veh, true, false);
        }

        internal static void RemovePersistence(List<Vehicle> recoveredVehList)
        {
            if (Config.PersistentVehicles) return;
            for (int i = recoveredVehList.Count - 1; i >= 0; i--)
                recoveredVehList.ElementAt(i).IsPersistent = false;
        }
    }
}