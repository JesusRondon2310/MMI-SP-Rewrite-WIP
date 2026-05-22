using System.Collections.Generic;
using GTA;
using GTA.Native;

namespace MMI_SP.Insurance.Observer.Delivery
{
    internal static class UpdateIncomingHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void Execute(List<Tracking> incomingVehicles)
        {
            for (int i = incomingVehicles.Count - 1; i >= 0; i--)
            {
                Tracking incoming = incomingVehicles[i];

                if (incoming.vehicle.IsDead)
                {
                    Handler.CannotBringVehicle(incoming, DeliveryHelper.GetVehicleInsuranceCost(incoming.vehicle));
                    continue;
                }

                if (HasDriverArrived(incoming))
                {
                    incoming.driver.Task.LeaveVehicle();
                    Function.Call(Hash.RESET_PED_LAST_VEHICLE, incoming.driver);
                    RemoveDriver(incoming, incomingVehicles, i);
                    continue;
                }

                if (Game.GameTime - incoming.calledTime > (Config.BringVehicleTimeout * 60000))
                    Handler.CannotBringVehicle(incoming);
            }
        }

        private static bool HasDriverArrived(Tracking incoming)
        {
            if (!incoming.driver.IsInVehicle(incoming.vehicle)) return false;

            if (incoming.vehicle.Model.IsHelicopter)
                return incoming.vehicle.Speed <= 0.5f && IsNearGround(incoming.vehicle, 5.0f);

            if (incoming.vehicle.Model.IsPlane)
                return incoming.vehicle.Speed <= 5.0f && IsNearGround(incoming.vehicle, 10.0f);

            return IsNearDestination(incoming);
        }

        private static bool IsNearGround(Vehicle vehicle, float maxAltitude)
        {
            World.GetGroundHeight(vehicle.Position, out float groundZ, GetGroundHeightMode.Normal);
            return vehicle.Position.Z - groundZ <= maxAltitude;
        }

        private static bool IsNearDestination(Tracking incoming)
        {
            float distanceToDest = incoming.driver.Position.DistanceTo(incoming.destination);
            float altitudeDiffDest = incoming.driver.Position.Z - incoming.destination.Z;

            float distanceToPlayer = incoming.driver.Position.DistanceTo(Game.Player.Character.Position);
            float altitudeDiffPlayer = incoming.driver.Position.Z - Game.Player.Character.Position.Z;

            return (distanceToDest <= 5.0f && altitudeDiffDest <= 2.0f) ||
                   (distanceToPlayer <= 5.0f && altitudeDiffPlayer <= 2.0f);
        }

        private static void RemoveDriver(Tracking incoming, List<Tracking> incomingVehicles, int index)
        {
            incoming.driver.IsPersistent = false;
            incoming.driver.MarkAsNoLongerNeeded();
            incoming.driver.Task.Wander();
            incomingVehicles.RemoveAt(index);
        }
    }
}