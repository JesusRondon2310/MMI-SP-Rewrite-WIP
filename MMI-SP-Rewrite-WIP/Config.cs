using GTA.Math;
using MMI_SP.PatternMatching;
using System;
using System.IO;

namespace MMI_SP
{
    internal static class Config
    {
        // ==========================================
        // BLOQUE 1: Variables de la clase y creación del objeto
        // ==========================================
        internal static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory + "\\MMI";
        internal static readonly string BannerImage = BaseDir + "\\banner.png";
        internal static readonly string InsuranceImage = BaseDir + "\\insurance.png";
        public static float InsuranceMult { get; } = 1.0f;
        public static bool PersistentVehicles { get; } = true;
        public static Vector3 PlayerPos => new Vector3(-822.528f, -260.00f, 35.79341f);
        public static int BringVehicleTimeout => 5; // minutos

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        public static Result<bool> Initialize()
        {
            try
            {
                if (!Directory.Exists(BaseDir))
                {
                    Logger.Debug("Creando carpeta MMI...");
                    Directory.CreateDirectory(BaseDir);
                }

                if (!File.Exists(InsuranceImage))
                {
                    Logger.Debug("Creating insurance image file");
                    Properties.Resources.insurance.Save(InsuranceImage);
                }
                return new Ok<bool>(true);
            }
            catch (Exception ex)
            {
                return new Err<bool>(ex.Message);
            }
        }
    }
}