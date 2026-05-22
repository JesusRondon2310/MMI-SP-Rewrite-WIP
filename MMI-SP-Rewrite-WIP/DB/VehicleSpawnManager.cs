using GTA;
using GTA.Native;
using MMI_SP.PatternMatching;
using System;

namespace MMI_SP.DB
{
    internal static class VehicleSpawnManager
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

            // Identidad
            veh.Mods.LicensePlate = data.Plate;
            Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT_INDEX, veh, data.PlateStyle);

            // Colores
            veh.Mods.PrimaryColor = (VehicleColor)data.PrimaryColor;
            veh.Mods.SecondaryColor = (VehicleColor)data.SecondaryColor;
            veh.Mods.WindowTint = (VehicleWindowTint)data.WindowTint;

            veh.Mods.InstallModKit();

            // Aplicar mods primero (para que WheelType y WheelColor prevalezcan después)
            foreach (var mod in data.Mods)
            {
                if (!Enum.IsDefined(typeof(VehicleToggleModType), mod.Key))
                    Function.Call(Hash.SET_VEHICLE_MOD, veh, mod.Key, mod.Value, false);
            }

            foreach (VehicleToggleModType toggleType in Enum.GetValues(typeof(VehicleToggleModType)))
            {
                int modType = (int)toggleType;
                if (data.Mods.TryGetValue(modType, out int value) && value == 1)
                    veh.Mods[toggleType].IsInstalled = true;
            }

            // Ruedas y neumáticos (después de los mods para evitar que ModType 23 pise el tipo)
            veh.Mods.WheelType = (VehicleWheelType)data.WheelType;
            veh.Mods.RimColor = (VehicleColor)data.WheelColor;

            Function.Call(Hash.SET_ENTITY_AS_MISSION_ENTITY, veh, true, false);

            if (data.BulletproofTires)
                Function.Call(Hash.SET_VEHICLE_TYRES_CAN_BURST, veh, false);

            // Neumáticos personalizados
            if (data.CustomTires)
            {
                int wheelMod = data.Mods.TryGetValue(23, out int modValue) ? modValue : 0;
                Function.Call(Hash.SET_VEHICLE_MOD, veh, 23, wheelMod, true);
            }
            else if (data.Mods.ContainsKey(23))
            {
                Function.Call(Hash.SET_VEHICLE_MOD, veh, 23, data.Mods[23], false);
            }

            if (data.TireSmokeColor >= 0)
            {
                veh.Mods[VehicleToggleModType.TireSmoke].IsInstalled = true;
                int r = (data.TireSmokeColor >> 16) & 0xFF;
                int g = (data.TireSmokeColor >> 8) & 0xFF;
                int b = data.TireSmokeColor & 0xFF;
                Function.Call(Hash.SET_VEHICLE_TYRE_SMOKE_COLOR, veh, r, g, b);
            }

            // Neón
            if (data.NeonColor >= 0)
            {
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Left, data.NeonLeft);
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Right, data.NeonRight);
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Front, data.NeonFront);
                veh.Mods.SetNeonLightsOn(VehicleNeonLight.Back, data.NeonBack);
                int r = (data.NeonColor >> 16) & 0xFF;
                int g = (data.NeonColor >> 8) & 0xFF;
                int b = data.NeonColor & 0xFF;
                veh.Mods.NeonLightsColor = System.Drawing.Color.FromArgb(r, g, b);
            }

            if (data.IsLocked)
            {
                veh.LockStatus = VehicleLockStatus.CannotEnter;
                veh.IsAlarmSet = true;
            }

            return new Ok<Vehicle>(veh);
        }
    }
}