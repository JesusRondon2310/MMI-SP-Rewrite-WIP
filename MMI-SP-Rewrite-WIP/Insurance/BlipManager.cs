using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using MMI_SP.Helpers;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance
{
    internal static class BlipManager
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static void RemoveRecoverBlip(Vehicle veh, Dictionary<string, Blip> blipsToRemove)
        {
            string key = VehicleIdentifier.Get(veh);
            blipsToRemove.TryGetValue(key, out Blip vehicleBlip);

            if (vehicleBlip != null)
            {
                vehicleBlip.Delete();
                blipsToRemove.Remove(key);
            }
        }

        internal static void ClearAllBlips(Dictionary<string, Blip> blipsToRemove)
        {
            for (int i = blipsToRemove.Count - 1; i >= 0; i--)
            {
                Blip toDel = blipsToRemove.ElementAt(i).Value;
                if (toDel != null && toDel.Exists()) toDel.Delete();
            }
        }

        internal static Result<Blip> AddVehicleBlip(Vehicle veh, bool flashing = false)
        {
            if (veh == null || !veh.Exists()) return new Err<Blip>("El vehículo no existe.");

            BlipSprite sprite = BlipSprite.PersonalVehicleCar;
            BlipColor color = BlipColor.White;

            if (veh.Model.IsBike || veh.Model.IsBicycle)
            {
                sprite = BlipSprite.PersonalVehicleBike;
                color = BlipColor.NetPlayer1;
            }
            else if (veh.Model == VehicleHash.Rhino || veh.Model == VehicleHash.Khanjali)
            {
                sprite = BlipSprite.Tank;
                color = BlipColor.Orange;
            }
            else if (veh.Model.IsHelicopter)
            {
                sprite = BlipSprite.Helicopter;
                color = BlipColor.Purple;
            }
            else if (veh.Model.IsPlane)
            {
                sprite = BlipSprite.ArmsTraffickingAir;
                color = BlipColor.Yellow;
            }
            else if (veh.Model.IsBoat)
            {
                sprite = BlipSprite.Speedboat;
                color = BlipColor.Blue;
            }
            else if (veh.ClassType == VehicleClass.Military)
            {
                sprite = BlipSprite.GunCar;
                color = BlipColor.Green;
            }

            Blip blip = veh.AddBlip();
            if (blip == null || !blip.Exists()) return new Err<Blip>("No se pudo crear el blip del vehículo.");

            blip.Sprite = sprite;
            blip.Color = color;
            blip.Name = "Vehículo asegurado";
            blip.IsShortRange = false;
            blip.IsFlashing = flashing;

            if (sprite == BlipSprite.ArmsTraffickingAir ||
                sprite == BlipSprite.Tank ||
                sprite == BlipSprite.Speedboat ||
                sprite == BlipSprite.GunCar)
                blip.Rotation = (int)veh.Rotation.Z;

            return new Ok<Blip>(blip);
        }
    }
}