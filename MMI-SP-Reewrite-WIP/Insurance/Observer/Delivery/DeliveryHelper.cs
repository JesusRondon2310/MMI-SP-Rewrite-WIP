using GTA;
using GTA.Math;
using MMI_SP.Insurance.Policies;

namespace MMI_SP.Insurance.Observer.Delivery
{
    internal static class DeliveryHelper
    {
        internal static int GetVehicleInsuranceCost(Vehicle veh) => Calculator.GetCost(veh);

        internal static EntityPosition GetVehicleSpawnLocation(Vector3 playerPos)
        {
            Vector3 forward = Game.Player.Character.ForwardVector;
            Vector3 spawnPos = playerPos + (forward * 5.0f);
            float heading = Game.Player.Character.Heading;
            return new EntityPosition(spawnPos, heading);
        }

        internal static EntityPosition GetVehicleRecoverNode(Vehicle veh)
        {
            Vector3 pos = Config.PlayerPos;
            return new EntityPosition(pos, 0.0f);
        }
    }
}