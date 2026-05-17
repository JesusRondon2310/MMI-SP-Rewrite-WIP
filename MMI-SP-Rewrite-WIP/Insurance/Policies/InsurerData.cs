using System.Collections.Generic;
using System.Linq;
using GTA;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Policies
{
    internal static class InsurerData
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<string> ValidateVehicle(List<VehicleData> vehicles, Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<string>("El vehículo no existe.");
            string id = VehicleIdentifier.Get(veh);
            if (vehicles.Any(v => v.Id == id)) return new Err<string>("El vehículo ya está asegurado.");
            return new Ok<string>(id);
        }

        internal static Result<VehicleData> FindVehicle(List<VehicleData> vehicles, string vehicleId)
        {
            var vehicle = vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle == null) return new Err<VehicleData>("Vehículo no encontrado.");
            return new Ok<VehicleData>(vehicle);
        }

        internal static Result<bool> MarkAsDestroyed(List<VehicleData> vehicles, string vehicleId)
        {
            return FindVehicle(vehicles, vehicleId)
                .AndThen(vehicle =>
                {
                    var updated = new VehicleData(
                        vehicle.Id, vehicle.ModelName, vehicle.Plate,
                        vehicle.PrimaryColor, vehicle.SecondaryColor, true,
                        vehicle.WindowTint, vehicle.WheelType, vehicle.WheelColor, vehicle.TireSmokeColor,
                        vehicle.PosX, vehicle.PosY, vehicle.PosZ, vehicle.Heading);
                    vehicles.Remove(vehicle);
                    vehicles.Add(updated);
                    Logger.Debug($"Flag IsDestroyed cambiado a true para {vehicleId}");
                    return new Ok<bool>(true);
                });
        }

        internal static Result<VehicleData> RecoverVehicle(List<VehicleData> vehicles, string vehicleId)
        {
            return FindVehicle(vehicles, vehicleId)
                .AndThen<VehicleData>(vehicle =>
                {
                    if (!vehicle.IsDestroyed) return new Err<VehicleData>("El vehículo no está destruido.");
                    return new Ok<VehicleData>(vehicle);
                });
        }

        internal static Result<bool> UpdateVehicleData(List<VehicleData> vehicles, Vehicle veh)
        {
            string id = VehicleIdentifier.Get(veh);
            return FindVehicle(vehicles, id)
                .AndThen(existing => VehicleDataFactory.CreateFrom(veh, id)
                    .AndThen(updated =>
                    {
                        var preserved = new VehicleData(
                            updated.Id, updated.ModelName, updated.Plate,
                            updated.PrimaryColor, updated.SecondaryColor, existing.IsDestroyed,
                            updated.WindowTint, updated.WheelType, updated.WheelColor, updated.TireSmokeColor,
                            updated.PosX, updated.PosY, updated.PosZ, updated.Heading);
                        vehicles.Remove(existing);
                        vehicles.Add(preserved);
                        return Repository.Save(vehicles);
                    })
                );
        }

        internal static Result<bool> UpdateVehicleDataById(List<VehicleData> vehicles, VehicleData updatedData)
        {
            return FindVehicle(vehicles, updatedData.Id)
                .AndThen(vehicle =>
                {
                    vehicles.Remove(vehicle);
                    vehicles.Add(updatedData);
                    return Repository.Save(vehicles);
                });
        }
    }
}