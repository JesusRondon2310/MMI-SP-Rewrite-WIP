using System.Collections.Generic;
using GTA;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance
{
    internal class Insurer
    {
        private List<string> _insuredVehicles;

        internal void LoadFrom(List<string> list) => _insuredVehicles = list;

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
            return _insuredVehicles.Contains(VehicleIdentifier.Get(veh));
        }

        internal Result<bool> Insure(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return new Err<bool>("El vehículo no existe.");
            string id = VehicleIdentifier.Get(veh);
            if (_insuredVehicles.Contains(id)) return new Err<bool>("El vehículo ya está asegurado.");

            _insuredVehicles.Add(id);
            Logger.Debug($"Vehículo asegurado: {id}");
            return Repository.Save(_insuredVehicles);
        }

        internal List<string> GetInsuredList() => new List<string>(_insuredVehicles);

        internal Result<bool> Cancel(string vehicleId)
        {
            if (!_insuredVehicles.Remove(vehicleId))
                return new Err<bool>("Vehículo no encontrado en la lista de asegurados.");

            Logger.Debug($"Vehículo cancelado: {vehicleId}");
            return Repository.Save(_insuredVehicles);
        }
    }
}