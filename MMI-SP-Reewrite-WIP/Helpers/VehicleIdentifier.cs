using GTA;
using System;

namespace MMI_SP.Helpers
{
    internal static class VehicleIdentifier
    {
        /// <summary>
        /// Devuelve el identificador único del vehículo (hashModelo_matrícula).
        /// </summary>
        public static string Get(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return string.Empty;
            return $"{veh.Model.Hash}_{veh.Mods.LicensePlate}";
        }

        public static string GetRandomNumberPlate()
        {
            Random rnd = new Random();
            string plate = "";
            for (int i = 0; i < 8; i++)
                plate += rnd.Next(0, 10).ToString();
            return plate;
        }
    }
}