using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Observer
{
    internal static class VehiclePersistence
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<Vehicle> SpawnVehicle(VehicleData data)
        {
            if (data == null) return new Err<Vehicle>("Datos del vehículo no válidos.");

            Model model = new Model(data.ModelName);
            Vehicle veh = World.CreateVehicle(model, new GTA.Math.Vector3(data.PosX, data.PosY, data.PosZ), data.Heading);

            if (veh == null || !veh.Exists()) return new Err<Vehicle>("No se pudo crear el vehículo.");

            veh.Mods.LicensePlate = data.Plate;
            veh.Mods.PrimaryColor = (VehicleColor)data.PrimaryColor;
            veh.Mods.SecondaryColor = (VehicleColor)data.SecondaryColor;
            veh.Mods.WindowTint = (VehicleWindowTint)data.WindowTint;
            veh.Mods.WheelType = (VehicleWheelType)data.WheelType;
            veh.Mods.RimColor = (VehicleColor)data.WheelColor;

            // Aplicar persistencia de sesión con el native más fiable
            Function.Call(Hash.SET_ENTITY_AS_MISSION_ENTITY, veh, true, false);

            return new Ok<Vehicle>(veh);
        }
    }
}