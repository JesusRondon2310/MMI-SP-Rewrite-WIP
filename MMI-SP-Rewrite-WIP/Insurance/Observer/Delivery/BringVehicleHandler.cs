using System.Collections.Generic;
using GTA;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Observer.Delivery
{
    internal static class BringVehicleHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void Execute(
            Vehicle veh, int cost, bool instant, bool recoveredVehicle,
            List<Vehicle> recoveredVehList, Dictionary<string, Blip> blipsToRemove,
            List<Tracking> incomingVehicles)
        {
            if (!veh.Exists()) return;

            CancelIfIncoming(veh, incomingVehicles, blipsToRemove);

            if (instant || IsHydra(veh))
                DeliverInstant(veh, recoveredVehicle, blipsToRemove);
            else
                ScheduleDelivery(veh, cost, recoveredVehicle, incomingVehicles, blipsToRemove);
        }

        private static void CancelIfIncoming(Vehicle veh, List<Tracking> incomingVehicles, Dictionary<string, Blip> blipsToRemove)
        {
            foreach (Tracking incoming in incomingVehicles)
            {
                if (incoming.vehicle != veh) continue;

                incoming.driver.Task.ClearAllImmediately();
                incoming.driver.IsPersistent = false;
                incoming.driver.Delete();

                if (!incoming.recovered)
                    BlipManager.RemoveRecoverBlip(incoming.vehicle, blipsToRemove);
                else
                    incoming.vehicle.Repair();
            }
        }

        private static bool IsHydra(Vehicle veh) => veh.Model.Hash == (int)AtHashValue.FromString("HYDRA");

        private static void DeliverInstant(Vehicle veh, bool recoveredVehicle, Dictionary<string, Blip> blipsToRemove)
        {
            if (veh.Model.IsBoat)
            {
                Tracking.BringBoat(veh);
                return;
            }

            EntityPosition pos = DeliveryHelper.GetVehicleSpawnLocation(Game.Player.Character.Position);
            veh.Position = pos.Position;
            veh.Heading = pos.Heading;

            if (!recoveredVehicle) AddBlip(veh, blipsToRemove);
        }

        private static void ScheduleDelivery(Vehicle veh, int cost, bool recoveredVehicle, List<Tracking> incomingVehicles, Dictionary<string, Blip> blipsToRemove)
        {
            Result<Tracking> result = null;

            if (veh.Model.IsCargobob || veh.Model.IsHelicopter)
                result = Tracking.BringHelicopter(veh, cost, recoveredVehicle);
            else if (veh.Model.IsPlane)
                result = Tracking.BringPlane(veh, cost, recoveredVehicle);
            else if (veh.Model.IsBoat)
                Tracking.BringBoat(veh);
            else
                result = Tracking.BringVehicle(veh, cost, recoveredVehicle);

            // Solo añadir si la creación fue exitosa (BringBoat no devuelve Result)
            if (result != null && result is Ok<Tracking> ok) incomingVehicles.Add(ok.Value);

            if (!recoveredVehicle) AddBlip(veh, blipsToRemove);
        }

        private static void AddBlip(Vehicle veh, Dictionary<string, Blip> blipsToRemove)
        {
            string key = VehicleIdentifier.Get(veh);
            if (blipsToRemove.TryGetValue(key, out Blip old) && old != null && old.Exists()) old.Delete();

            var result = BlipManager.AddVehicleBlip(veh);
            if (result is Ok<Blip> ok) blipsToRemove[key] = ok.Value;
        }
    }
}