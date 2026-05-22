using System.Collections.Generic;
using System.Linq;
using GTA;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.DB
{
    public static class Core
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal static List<VehicleData> Vehicles = new List<VehicleData>();

        // ==========================================
        // BLOQUE 2: Persistencia
        // ==========================================
        public static Result<bool> Load()
        {
            return JSONHandler.Load().match<Result<bool>>(
                onOk: data =>
                {
                    Vehicles = data;
                    return new Ok<bool>(true);
                },
                onErr: error => new Err<bool>(error)
            );
        }

        internal static Result<bool> Save() => JSONHandler.Save(Vehicles);

        // ==========================================
        // BLOQUE 3: CRUD
        // ==========================================
        public static bool IsInsured(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return false;
            string id = VehicleIdentifier.Get(veh);
            return Vehicles.Any(v => v.Id == id);
        }

        public static Option<VehicleData> FindVehicle(string vehicleId)
            => InsuredVehiclesData.FindVehicle(vehicleId);

        public static Result<bool> Add(VehicleData data)
        {
            Vehicles.Add(data);
            return Save();
        }

        public static Result<bool> Remove(string vehicleId)
        {
            var vehicle = Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle == null) return new Err<bool>("Vehículo no encontrado.");
            Vehicles.Remove(vehicle);
            return Save();
        }

        public static Result<bool> Update(VehicleData updatedData)
        {
            var existing = Vehicles.FirstOrDefault(v => v.Id == updatedData.Id);
            if (existing == null) return new Err<bool>("Vehículo no encontrado.");
            Vehicles.Remove(existing);
            Vehicles.Add(updatedData);
            return Save();
        }

        // ==========================================
        // BLOQUE 4: Consultas
        // ==========================================
        internal static List<VehicleData> GetAll() => new List<VehicleData>(Vehicles);

        internal static List<string> GetInsuredIds()
            => Vehicles.Where(v => !v.IsDestroyed).Select(v => v.Id).ToList();

        internal static List<string> GetDestroyedIds()
            => Vehicles.Where(v => v.IsDestroyed).OrderByDescending(v => v.DestroyedAt).Take(5).Select(v => v.Id).ToList();
    }
}