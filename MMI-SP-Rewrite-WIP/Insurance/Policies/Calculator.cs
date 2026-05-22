using GTA;
using MMI_SP.PatternMatching;

namespace MMI_SP.Insurance.Policies
{
    public static class Calculator
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static int GetCost(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return 0;
            return (int)(BaseCost(veh) * Config.InsuranceMult);
        }

        public static int GetRecoverCost(string vehicleId)
        {
            return DB.Core.FindVehicle(vehicleId).match<int>(
                onSome: data =>
                {
                    Model model = new Model(data.ModelName);
                    return (int)(BaseCost(model) * Config.RecoverMult);
                },
                onNone: () => 0
            );
        }

        private static int BaseCost(Vehicle veh) => BaseCost(veh.Model);

        private static int BaseCost(Model model)
        {
            int baseCost = 500;
            if (model.IsHelicopter || model.IsPlane) baseCost += 5000;
            else if (model.IsBoat) baseCost += 2000;
            else if (model.IsBike || model.IsQuadBike) baseCost += 300;
            else baseCost += 1000;
            return baseCost;
        }
    }
}