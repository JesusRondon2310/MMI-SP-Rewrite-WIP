using GTA;
using MMI_SP.Debug;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Observer;
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
        internal static Result<Vehicle> RecoverVehicle(string vehicleId, Insurer insurer)
        {
            if (string.IsNullOrEmpty(vehicleId)) return new Err<Vehicle>("ID de vehículo no válido.");

            // 1. Cambiar el estado en la base de datos y obtener los datos actualizados
            var recoverResult = insurer.RecoverVehicle(vehicleId);
            if (recoverResult is Err<DB.VehicleData> err) return new Err<Vehicle>(err.Message);
            DB.VehicleData data = recoverResult.unwrap_or(null);

            // 2. Elegir un nodo de spawn adecuado
            Model model = new Model(data.ModelName);
            Vehicle tempVeh = World.CreateVehicle(model, new GTA.Math.Vector3(), 0f);
            if (tempVeh == null || !tempVeh.Exists()) return new Err<Vehicle>("No se pudo crear el modelo base para elegir nodo.");

            var nodeResult = Coordinates.GetRecoverNode(tempVeh);
            tempVeh.Delete();

            if (nodeResult is Err<EntityPosition> nodeErr)
                return new Err<Vehicle>($"No se pudo obtener nodo de spawn: {nodeErr.Message}");
            EntityPosition spawnPos = ((Ok<EntityPosition>)nodeResult).Value;

            // 3. Actualizar la posición en el VehicleData con las coordenadas del nodo elegido
            var updatedData = new DB.VehicleData(
                data.Id, data.ModelName, data.Plate,
                data.PrimaryColor, data.SecondaryColor, false,
                windowTint: data.WindowTint,
                wheelType: data.WheelType,
                wheelColor: data.WheelColor,
                tireSmokeColor: data.TireSmokeColor,
                neonLeft: data.NeonLeft,
                neonRight: data.NeonRight,
                neonFront: data.NeonFront,
                neonBack: data.NeonBack,
                neonColor: data.NeonColor,
                bulletproofTires: data.BulletproofTires,
                posX: spawnPos.Position.X, posY: spawnPos.Position.Y,
                posZ: spawnPos.Position.Z, heading: spawnPos.Heading,
                mods: data.Mods,
                destroyedAt: data.DestroyedAt,
                plateStyle: data.PlateStyle,
                customTires: data.CustomTires);

            DB.Core.Update(updatedData).match<bool>(
                onOk: _ => true,
                onErr: error =>
                {
                    Logger.Error($"Error al actualizar posición de spawn en DB: {error}");
                    return false;
                }
            );

            // 4. Spawnear el vehículo real en el mundo
            var spawnResult = DB.VehicleSpawnManager.SpawnVehicle(updatedData);
            if (spawnResult is Err<Vehicle> spawnErr) return spawnErr;

            Vehicle spawned = ((Ok<Vehicle>)spawnResult).Value;

            // 5. Eliminar cualquier blip existente y poner uno titilante
            spawned.AttachedBlip?.Delete();

            BlipManager.AddVehicleBlip(spawned, flashing: true).match<bool>(
                onOk: blip =>
                {
                    string key = VehicleIdentifier.Get(spawned);
                    Observer.Manager.BlipsToRemove[key] = blip;
                    return true;
                },
                onErr: error =>
                {
                    Logger.Error($"Error al crear blip titilante: {error}");
                    return false;
                }
            );

            return new Ok<Vehicle>(spawned);
        }
    }
}