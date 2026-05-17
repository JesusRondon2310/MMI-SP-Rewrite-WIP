using System.Collections.Generic;
using GTA;

namespace MMI_SP.Insurance.Observer.Delivery
{
    internal static class Handler
    {
        private static List<Tracking> _incomingVehicles = new List<Tracking>();

        internal static void BringVehicleToPlayer(
            Vehicle veh, int cost, bool instant, bool recovered,
            List<Vehicle> recoveredVehList, Dictionary<string, Blip> blipsToRemove)
        {
            BringVehicleHandler.Execute(veh, cost, instant, recovered, recoveredVehList, blipsToRemove, _incomingVehicles);
        }

        internal static void UpdateIncomingVehicles()
        {
            UpdateIncomingHandler.Execute(_incomingVehicles);
        }

        internal static void CannotBringVehicle(Tracking incoming, int refund = 0)
        {
            CannotBringHandler.Execute(incoming, refund, _incomingVehicles);
        }

        internal static List<Vehicle> GetBringableVehicles(List<Vehicle> recovered, List<Vehicle> insured)
        {
            return GetBringableHandler.Execute(recovered, insured);
        }
    }
}