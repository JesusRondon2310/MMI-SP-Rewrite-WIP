using GTA;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Observer.Delivery;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Recover
{
    internal static class Manager
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        /// <summary>
        /// Recupera un vehículo destruido a partir de su ID.
        /// Cambia su estado a no destruido, elige una posición de spawn y lo crea en el mundo.
        /// </summary>
        internal static Result<Vehicle> RecoverVehicle(string vehicleId, Insurer insurer)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<Vehicle>("ID de vehículo no válido.");

            // 1. Cambiar el estado en la base de datos y obtener los datos actualizados
            var recoverResult = insurer.RecoverVehicle(vehicleId);
            if (recoverResult is Err<VehicleData> err) return new Err<Vehicle>(err.Message);
            VehicleData data = ((Ok<VehicleData>)recoverResult).Value;

            // 2. Elegir un nodo de spawn adecuado
            Model model = new Model(data.ModelName);
            Vehicle tempVeh = World.CreateVehicle(model, new GTA.Math.Vector3(), 0f);
            if (tempVeh == null || !tempVeh.Exists()) return new Err<Vehicle>("No se pudo crear el modelo base para elegir nodo.");

            EntityPosition spawnPos = Coordinates.GetRecoverNode(tempVeh);
            tempVeh.Delete();

            // 3. Actualizar la posición en el VehicleData con las coordenadas del nodo elegido
            var updatedData = new VehicleData(
                data.Id, data.ModelName, data.Plate,
                data.PrimaryColor, data.SecondaryColor, false,
                data.WindowTint, data.WheelType, data.WheelColor, data.TireSmokeColor,
                spawnPos.Position.X, spawnPos.Position.Y, spawnPos.Position.Z, spawnPos.Heading);
            insurer.UpdateVehicleDataById(updatedData);

            // 4. Spawnear el vehículo real en el mundo
            return Observer.VehiclePersistence.SpawnVehicle(updatedData);
        }
    }
}