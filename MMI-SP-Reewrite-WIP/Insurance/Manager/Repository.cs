using System;
using System.Collections.Generic;
using System.IO;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance
{
    internal static class Repository
    {
        internal static string DatabasePath => Path.Combine(Config.BaseDir, "asegurados.txt");

        internal static Result<List<string>> Load()
        {
            try
            {
                if (File.Exists(DatabasePath))
                {
                    var list = new List<string>(File.ReadAllLines(DatabasePath));
                    Logger.Debug($"Loaded {list.Count} insured vehicles.");
                    return new Ok<List<string>>(list);
                }
                Logger.Debug("No insured database found. Starting with empty list.");
                return new Ok<List<string>>(new List<string>());
            }
            catch (Exception ex)
            {
                return new Err<List<string>>(ex.Message);
            }
        }

        internal static Result<bool> Save(List<string> insuredVehicles)
        {
            try
            {
                File.WriteAllLines(DatabasePath, insuredVehicles);
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