using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Policies
{
    internal class Insurer
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private List<VehicleData> _insuredVehicles;
        internal static Insurer Instance { get; private set; }
        internal List<VehicleData> GetInsuredListFull() => new List<VehicleData>(_insuredVehicles);

        internal Insurer()
        {
            Instance = this;
        }

        // ==========================================
        // BLOQUE 2: Funciones públicas
        // ==========================================
        internal void LoadFrom(List<VehicleData> list) => _insuredVehicles = list;

        public static bool IsInsurable(Vehicle veh)
        {
            if (veh == null || !veh.IsAlive) return false;
            return !veh.Model.IsTrain &&
                   (veh.Model.IsCar || veh.Model.IsBike || veh.Model.IsQuadBike ||
                    veh.Model.IsHelicopter || veh.Model.IsPlane || veh.Model.IsBoat);
        }

        internal bool IsInsured(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return false;
            string id = VehicleIdentifier.Get(veh);
            return _insuredVehicles.Any(v => v.Id == id);
        }

        internal Result<bool> Insure(Vehicle veh)
        {
            return ValidateVehicle(veh)
                .AndThen(id => VehicleDataFactory.CreateFrom(veh, id))
                .AndThen(data =>
                {
                    _insuredVehicles.Add(data);
                    Logger.Debug($"Vehículo asegurado: {data.Id}");
                    return Repository.Save(_insuredVehicles);
                });
        }

        internal List<string> GetInsuredList() => _insuredVehicles.Select(v => v.Id).ToList();
        internal List<string> GetDestroyedList() => _insuredVehicles.Where(v => v.IsDestroyed).Select(v => v.Id).ToList();

        internal Result<bool> Cancel(string vehicleId)
        {
            return FindVehicle(vehicleId)
                .AndThen(vehicle =>
                {
                    _insuredVehicles.Remove(vehicle);
                    Logger.Debug($"Vehículo cancelado: {vehicleId}");
                    return Repository.Save(_insuredVehicles);
                });
        }

        internal Result<bool> MarkAsDestroyed(string vehicleId)
        {
            return FindVehicle(vehicleId)
                .AndThen(vehicle =>
                {
                    var updated = new VehicleData(
                        vehicle.Id, vehicle.ModelName, vehicle.Plate,
                        vehicle.PrimaryColor, vehicle.SecondaryColor, true,
                        vehicle.WindowTint, vehicle.WheelType, vehicle.WheelColor, vehicle.TireSmokeColor,
                        vehicle.PosX, vehicle.PosY, vehicle.PosZ, vehicle.Heading);
                    _insuredVehicles.Remove(vehicle);
                    _insuredVehicles.Add(updated);
                    Logger.Debug($"Flag IsDestroyed cambiado a true para {vehicleId}");
                    return new Ok<bool>(true);
                });
        }

        internal Result<bool> UpdateVehicleData(Vehicle veh)
        {
            string id = VehicleIdentifier.Get(veh);
            return FindVehicle(id)
                .AndThen(vehicle => VehicleDataFactory.CreateFrom(veh, id)
                    .AndThen(updated =>
                    {
                        // Preservar el estado de destrucción original
                        var preserved = new VehicleData(
                            updated.Id, updated.ModelName, updated.Plate,
                            updated.PrimaryColor, updated.SecondaryColor, vehicle.IsDestroyed, // ← aquí se mantiene el valor anterior
                            updated.WindowTint, updated.WheelType, updated.WheelColor, updated.TireSmokeColor,
                            updated.PosX, updated.PosY, updated.PosZ, updated.Heading);
                        _insuredVehicles.Remove(vehicle);
                        _insuredVehicles.Add(preserved);
                        return Repository.Save(_insuredVehicles);
                    })
                );
        }

        internal Result<VehicleData> RecoverVehicle(string vehicleId)
        {
            return FindVehicle(vehicleId)
                .AndThen<VehicleData>(vehicle =>
                {
                    if (!vehicle.IsDestroyed) return new Err<VehicleData>("El vehículo no está destruido.");
                    return new Ok<VehicleData>(vehicle); // Devolvemos los datos originales para que el Manager los modifique
                });
        }

        internal Result<bool> UpdateVehicleDataById(VehicleData updatedData)
        {
            return FindVehicle(updatedData.Id)
                .AndThen(vehicle =>
                {
                    _insuredVehicles.Remove(vehicle);
                    _insuredVehicles.Add(updatedData);
                    return Repository.Save(_insuredVehicles);
                });
        }

        // ==========================================
        // BLOQUE 3: Funciones privadas (validaciones)
        // ==========================================
        private Result<string> ValidateVehicle(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<string>("El vehículo no existe.");
            string id = VehicleIdentifier.Get(veh);
            if (_insuredVehicles.Any(v => v.Id == id)) return new Err<string>("El vehículo ya está asegurado.");
            return new Ok<string>(id);
        }

        private Result<VehicleData> FindVehicle(string vehicleId)
        {
            var vehicle = _insuredVehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle == null) return new Err<VehicleData>("Vehículo no encontrado.");
            return new Ok<VehicleData>(vehicle);
        }

    }
}