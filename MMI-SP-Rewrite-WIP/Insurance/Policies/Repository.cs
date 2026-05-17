using System;
using System.Collections.Generic;
using System.IO;
using MMI_SP.PatternMatching;
using Newtonsoft.Json;

namespace MMI_SP.Insurance.Policies
{
    internal static class Repository
    {
        internal static string DatabasePath => Path.Combine(Config.BaseDir, "db.json");

        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        internal static Result<List<VehicleData>> Load()
        {
            try
            {
                if (File.Exists(DatabasePath))
                {
                    string json = File.ReadAllText(DatabasePath);
                    var list = JsonConvert.DeserializeObject<List<VehicleData>>(json);
                    Logger.Debug($"Loaded {list?.Count ?? 0} insured vehicles.");
                    return new Ok<List<VehicleData>>(list ?? new List<VehicleData>());
                }
                Logger.Debug("No insured database found. Starting with empty list.");
                return new Ok<List<VehicleData>>(new List<VehicleData>());
            }
            catch (Exception ex)
            {
                return new Err<List<VehicleData>>(ex.Message);
            }
        }

        internal static Result<bool> Save(List<VehicleData> insuredVehicles)
        {
            try
            {
                string json = JsonConvert.SerializeObject(insuredVehicles, Formatting.Indented);
                File.WriteAllText(DatabasePath, json);
                Logger.Debug("Insured database saved.");
                return new Ok<bool>(true);
            }
            catch (Exception ex)
            {
                return new Err<bool>(ex.Message);
            }
        }
    }
}