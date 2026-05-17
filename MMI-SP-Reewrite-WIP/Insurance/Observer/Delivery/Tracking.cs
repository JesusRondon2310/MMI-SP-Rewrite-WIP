using GTA;
using GTA.Math;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Observer.Delivery
{
    internal class Tracking
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal Vehicle vehicle;
        internal Ped driver;
        internal int price;
        internal Vector3 destination;
        internal int calledTime;
        internal bool recovered;
        internal EntityPosition originalPosition;

        // ==========================================
        // BLOQUE 2: Factory methods estáticos
        // ==========================================
        internal static Result<Tracking> BringVehicle(Vehicle veh, int cost, bool isRecovered)
        {
            return CreateIncoming(veh, cost, isRecovered, true);
        }

        internal static Result<Tracking> BringHelicopter(Vehicle veh, int cost, bool isRecovered)
        {
            return CreateIncoming(veh, cost, isRecovered, false);
        }

        internal static Result<Tracking> BringPlane(Vehicle veh, int cost, bool isRecovered)
        {
            return CreateIncoming(veh, cost, isRecovered, false);
        }

        internal static void BringBoat(Vehicle veh)
        {
            EntityPosition pos = DeliveryHelper.GetVehicleSpawnLocation(Game.Player.Character.Position);
            veh.Position = pos.Position;
            veh.Heading = pos.Heading;
        }

        // ==========================================
        // BLOQUE 3: Métodos privados
        // ==========================================
        private static Result<Tracking> CreateIncoming(Vehicle veh, int cost, bool isRecovered, bool driveToDestination)
        {
            if (veh == null || !veh.Exists()) return new Err<Tracking>("El vehículo no existe.");

            Tracking incoming = new Tracking
            {
                vehicle = veh,
                price = cost,
                recovered = isRecovered,
                originalPosition = new EntityPosition(veh.Position, veh.Heading),
                destination = Game.Player.Character.Position,
                calledTime = Game.GameTime
            };

            incoming.driver = World.CreatePed(PedHash.Methhead01AMY, veh.Position, veh.Heading);
            if (incoming.driver == null || !incoming.driver.Exists()) return new Err<Tracking>("No se pudo crear el conductor.");

            if (driveToDestination)
                incoming.driver.Task.DriveTo(veh, incoming.destination, 5.0f, VehicleDrivingFlags.DrivingModeStopForVehicles, 20.0f);

            return new Ok<Tracking>(incoming);
        }
    }
}