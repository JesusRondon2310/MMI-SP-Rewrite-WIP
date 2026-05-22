using GTA.Math;
using MMI_SP.PatternMatching;
using System;
using System.Collections.Generic;
using System.IO;

namespace MMI_SP
{
    internal static class Config
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory + "\\MMI";
        internal static readonly string BannerImage = BaseDir + "\\banner.png";
        internal static readonly string InsuranceImage = BaseDir + "\\insurance.png";
        internal static readonly string IniPath = BaseDir + "\\config.ini";

        public static float InsuranceMult { get; set; } = 1.0f;
        public static float RecoverMult { get; set; } = 1.0f;
        public static bool PersistentVehicles { get; set; } = true;
        public static Vector3 PlayerPos => new Vector3(-822.528f, -260.00f, 35.79341f);

        public static int iFruitVolume { get; set; } = 25;
        public static bool CaniFruitInsure { get; set; } = true;
        public static bool CaniFruitCancel { get; set; } = true;
        public static bool CaniFruitRecover { get; set; } = true;

        public static int BringVehicleBasePrice { get; set; } = 200;
        public static bool BringVehicleInstant { get; set; } = false;
        public static int BringVehicleRadius { get; set; } = 100;
        public static int BringVehicleTimeout { get; set; } = 5;

        private static readonly Dictionary<string, string> _settings = new Dictionary<string, string>();

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static Result<bool> Initialize()
        {
            try
            {
                if (!Directory.Exists(BaseDir))
                    Directory.CreateDirectory(BaseDir);

                if (!File.Exists(InsuranceImage)) File.Copy("Resources/insurance.png", InsuranceImage);

                LoadFromSettings();
                return new Ok<bool>(true);
            }
            catch (Exception ex)
            {
                return new Err<bool>(ex.Message);
            }
        }

        internal static void UpdateValue(string key, object value)
        {
            switch (key)
            {
                case "InsuranceCostMultiplier": InsuranceMult = Convert.ToSingle(value); break;
                case "RecoverCostMultiplier": RecoverMult = Convert.ToSingle(value); break;
                case "PersistentInsuredVehicles": PersistentVehicles = Convert.ToBoolean(value); break;
                case "PhoneVolume": iFruitVolume = Convert.ToInt32(value); break;
                case "CaniFruitInsure": CaniFruitInsure = Convert.ToBoolean(value); break;
                case "CaniFruitCancel": CaniFruitCancel = Convert.ToBoolean(value); break;
                case "CaniFruitRecover": CaniFruitRecover = Convert.ToBoolean(value); break;
                case "BringVehicleBasePrice": BringVehicleBasePrice = Convert.ToInt32(value); break;
                case "BringVehicleInstant": BringVehicleInstant = Convert.ToBoolean(value); break;
                case "BringVehicleRadius": BringVehicleRadius = Convert.ToInt32(value); break;
                case "BringVehicleTimeout": BringVehicleTimeout = Convert.ToInt32(value); break;
            }
        }

        internal static void SetSetting(string section, string key, string value)
        {
            string composite = $"{section}.{key}";
            _settings[composite] = value;
            SaveSettings();
        }

        internal static string GetSetting(string section, string key, string defaultValue = "")
        {
            string composite = $"{section}.{key}";
            return _settings.TryGetValue(composite, out string value) ? value : defaultValue;
        }

        private static void LoadFromSettings()
        {
            if (!File.Exists(IniPath)) return;

            foreach (string line in File.ReadAllLines(IniPath))
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith(";") || trimmed.StartsWith("#"))
                    continue;

                string[] parts = trimmed.Split('=');
                if (parts.Length != 2) continue;

                string[] sectionKey = parts[0].Trim().Split('.');
                if (sectionKey.Length != 2) continue;

                string section = sectionKey[0].Trim();
                string key = sectionKey[1].Trim();
                string value = parts[1].Trim();

                string composite = $"{section}.{key}";
                _settings[composite] = value;

                switch (key)
                {
                    case "InsuranceCostMultiplier": InsuranceMult = Convert.ToSingle(value); break;
                    case "RecoverCostMultiplier": RecoverMult = Convert.ToSingle(value); break;
                    case "PersistentInsuredVehicles": PersistentVehicles = Convert.ToBoolean(value); break;
                    case "PhoneVolume": iFruitVolume = Convert.ToInt32(value); break;
                    case "CaniFruitInsure": CaniFruitInsure = Convert.ToBoolean(value); break;
                    case "CaniFruitCancel": CaniFruitCancel = Convert.ToBoolean(value); break;
                    case "CaniFruitRecover": CaniFruitRecover = Convert.ToBoolean(value); break;
                    case "BringVehicleBasePrice": BringVehicleBasePrice = Convert.ToInt32(value); break;
                    case "BringVehicleInstant": BringVehicleInstant = Convert.ToBoolean(value); break;
                    case "BringVehicleRadius": BringVehicleRadius = Convert.ToInt32(value); break;
                    case "BringVehicleTimeout": BringVehicleTimeout = Convert.ToInt32(value); break;
                }
            }
        }

        internal static void SaveSettings()
        {
            using (StreamWriter writer = new StreamWriter(IniPath, false))
            {
                writer.WriteLine("; MMI-SP Configuration");
                writer.WriteLine();
                foreach (KeyValuePair<string, string> entry in _settings)
                {
                    string[] sectionKey = entry.Key.Split('.');
                    if (sectionKey.Length == 2)
                        writer.WriteLine($"{sectionKey[0]}.{sectionKey[1]}={entry.Value}");
                }
            }
        }
    }
}