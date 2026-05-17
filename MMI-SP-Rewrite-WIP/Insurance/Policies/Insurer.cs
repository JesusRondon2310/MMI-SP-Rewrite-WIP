using System.Collections.Generic;
using System.Linq;
using GTA;
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
            return _insuredVehicles.Any(v => v.Id == VehicleIdentifier.Get(veh));
        }

        internal Result<bool> Insure(Vehicle veh)
        {
            return InsurerData.ValidateVehicle(_insuredVehicles, veh)
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
            return InsurerData.FindVehicle(_insuredVehicles, vehicleId)
                .AndThen(vehicle =>
                {
                    _insuredVehicles.Remove(vehicle);
                    Logger.Debug($"Vehículo cancelado: {vehicleId}");
                    return Repository.Save(_insuredVehicles);
                });
        }

        internal Result<bool> MarkAsDestroyed(string vehicleId)
            => InsurerData.MarkAsDestroyed(_insuredVehicles, vehicleId);

        internal Result<bool> UpdateVehicleData(Vehicle veh)
            => InsurerData.UpdateVehicleData(_insuredVehicles, veh);

        internal Result<VehicleData> RecoverVehicle(string vehicleId)
            => InsurerData.RecoverVehicle(_insuredVehicles, vehicleId);

        internal Result<bool> UpdateVehicleDataById(VehicleData updatedData)
            => InsurerData.UpdateVehicleDataById(_insuredVehicles, updatedData);
    }
}