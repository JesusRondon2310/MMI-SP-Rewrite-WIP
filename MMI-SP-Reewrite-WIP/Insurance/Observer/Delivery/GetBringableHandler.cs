using System.Collections.Generic;
using System.Linq;
using GTA;

namespace MMI_SP.Insurance.Observer.Delivery
{
    internal static class GetBringableHandler
    {
        internal static List<Vehicle> Execute(List<Vehicle> recoveredVehList, List<Vehicle> insuredVehList)
        {
            List<Vehicle> vehiclesToBring = new List<Vehicle>(recoveredVehList);
            foreach (Vehicle v in insuredVehList)
                if (!vehiclesToBring.Contains(v)) vehiclesToBring.Add(v);

            if (Game.Player.Character.CurrentVehicle != null &&
                vehiclesToBring.Contains(Game.Player.Character.CurrentVehicle))
                vehiclesToBring.Remove(Game.Player.Character.CurrentVehicle);

            return vehiclesToBring;
        }
    }
}