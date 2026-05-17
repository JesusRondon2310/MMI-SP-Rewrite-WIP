using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Policies
{
    internal static class VehicleDataFactory
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<VehicleData> CreateFrom(Vehicle veh, string id)
        {
            if (veh == null || !veh.Exists())
                return new Err<VehicleData>("El vehículo no existe.");
            if (string.IsNullOrEmpty(id))
                return new Err<VehicleData>("ID de vehículo no válido.");

            string modelName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            string plate = veh.Mods.LicensePlate;

            if (string.IsNullOrEmpty(plate))
                return new Err<VehicleData>("El vehículo no tiene placa.");

            int primaryColor = (int)veh.Mods.PrimaryColor;
            int secondaryColor = (int)veh.Mods.SecondaryColor;
            int windowTint = (int)veh.Mods.WindowTint;
            int wheelType = (int)veh.Mods.WheelType;
            int wheelColor = (int)veh.Mods.RimColor;
            int tireSmokeColor = GetTireSmokeColor(veh);

            float posX = veh.Position.X;
            float posY = veh.Position.Y;
            float posZ = veh.Position.Z;
            float heading = veh.Heading;

            return new Ok<VehicleData>(new VehicleData(id, modelName, plate, primaryColor, secondaryColor, false, windowTint, wheelType, wheelColor, 
                tireSmokeColor, posX, posY, posZ, heading));
        }

        private static int GetTireSmokeColor(Vehicle veh)
        {
            return ValidateTireSmoke(veh).Match(
                onOk: color => color,
                onErr: _ => 0
            );
        }

        private static Result<int> ValidateTireSmoke(Vehicle veh)
        {
            bool hasTireSmoke = Function.Call<int>(Hash.GET_VEHICLE_MOD, veh, 20) != -1;
            if (!hasTireSmoke) return new Err<int>("Vehículo sin mod de humo.");
            return new Ok<int>(Function.Call<int>(Hash.GET_VEHICLE_TYRE_SMOKE_COLOR, veh));
        }
    }
}