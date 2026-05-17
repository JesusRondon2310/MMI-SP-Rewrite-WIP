using System.Collections.Generic;
using GTA;
using GTA.Math;
using MMI_SP.Helpers;

namespace MMI_SP.Insurance.Observer.Delivery
{
    internal static class CannotBringHandler
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void Execute(Tracking incoming, int refund, List<Tracking> incomingVehicles)
        {
            NotifyFailure();
            Refund(incoming, refund);
            incoming.driver.Delete();
            RepairIfAlive(incoming);
            incomingVehicles.Remove(incoming);
        }

        private static void NotifyFailure()
        {
            Notification.Show("CHAR_MP_MORS_MUTUAL", "MORS MUTUAL INSURANCE",
                "Solicitando vehículo...", "No se pudo traer el vehículo.");
        }

        private static void Refund(Tracking incoming, int refund)
        {
            Game.Player.Money += (refund == 0) ? incoming.price : refund + incoming.price;
        }

        private static void RepairIfAlive(Tracking incoming)
        {
            if (incoming.vehicle.IsDead) return;

            if (incoming.originalPosition.Position != Vector3.Zero)
            {
                incoming.vehicle.Position = incoming.originalPosition.Position;
                incoming.vehicle.Heading = incoming.originalPosition.Heading;
            }
            else
            {
                EntityPosition pos = DeliveryHelper.GetVehicleRecoverNode(incoming.vehicle);
                incoming.vehicle.Position = pos.Position;
                incoming.vehicle.Heading = pos.Heading;
            }

            incoming.vehicle.IsEngineRunning = false;
            incoming.vehicle.Repair();
        }
    }
}