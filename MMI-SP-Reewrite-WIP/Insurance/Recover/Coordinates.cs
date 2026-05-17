using GTA;
using GTA.Math;
using GTA.Native;
using MMI_SP.Insurance.Observer.Delivery;
using System;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Recover
{
    internal static class Coordinates
    {
        // ==========================================
        // BLOQUE 1: Datos — Listas de nodos por tipo
        // ==========================================
        private static readonly List<EntityPosition> SpawnListVehicle = new List<EntityPosition>
        {
            new EntityPosition(new Vector3(-225.2716f, -1182.783f, 22.49698f), 2.3600f),
            new EntityPosition(new Vector3(-229.9406f, -1182.361f, 22.49209f), 6.1440f),
            new EntityPosition(new Vector3(-234.6615f, -1182.197f, 22.48984f), 355.5509f),
            new EntityPosition(new Vector3(-244.1168f, -1179.623f, 22.5177f), 308.1156f),
            new EntityPosition(new Vector3(-243.4413f, -1173.07f, 22.53329f), 271.4005f),
            new EntityPosition(new Vector3(-243.5148f, -1166.511f, 22.56954f), 242.3607f),
            new EntityPosition(new Vector3(-237.2427f, -1162.784f, 22.47172f), 183.7536f),
            new EntityPosition(new Vector3(-232.8058f, -1162.548f, 22.44885f), 182.2262f),
            new EntityPosition(new Vector3(-228.4865f, -1162.615f, 22.45386f), 181.4573f),
            new EntityPosition(new Vector3(-150.4142f, -1166.01f, 24.73805f), 177.0276f),
            new EntityPosition(new Vector3(-143.6111f, -1163.825f, 24.76486f), 160.3781f),
            new EntityPosition(new Vector3(-136.2873f, -1183.153f, 24.7363f), 78.20843f),
            new EntityPosition(new Vector3(-136.9411f, -1177.181f, 24.72224f), 102.267f),
            new EntityPosition(new Vector3(-246.5937f, -1150.561f, 22.62461f), 269.4836f),
            new EntityPosition(new Vector3(-238.7069f, -1150.786f, 22.62887f), 269.3971f),
            new EntityPosition(new Vector3(-232.8114f, -1150.434f, 22.54277f), 272.1211f),
            new EntityPosition(new Vector3(-211.5235f, -1150.303f, 22.55123f), 268.1985f),
            new EntityPosition(new Vector3(-198.5835f, -1150.331f, 22.54078f), 269.7671f)
        };

        private static readonly List<EntityPosition> SpawnListVehicleLong = new List<EntityPosition>
        {
            new EntityPosition(new Vector3(-157.9389f, -1162.761f, 24.11157f), 0.6600574f),
            new EntityPosition(new Vector3(-236.0531f, -1149.395f, 23.04231f), 269.1866f),
            new EntityPosition(new Vector3(-174.2821f, -1149.661f, 23.17635f), 269.3501f),
            new EntityPosition(new Vector3(-200.4261f, -1182.882f, 23.1067f), 90.51575f)
        };

        private static readonly List<EntityPosition> SpawnListHeli = new List<EntityPosition>
        {
            new EntityPosition(new Vector3(-746.6312f, -1432.797f, 4.71605f), 231.0658f),
            new EntityPosition(new Vector3(-763.4095f, -1453.074f, 4.722716f), 234.3286f),
            new EntityPosition(new Vector3(-746.3437f, -1469.839f, 4.718675f), 322.4937f),
            new EntityPosition(new Vector3(-721.1809f, -1473.602f, 4.717093f), 49.87125f),
            new EntityPosition(new Vector3(-700.242f, -1447.846f, 4.71675f), 53.22678f),
            new EntityPosition(new Vector3(-723.8517f, -1442.887f, 4.716637f), 139.5879f)
        };

        private static readonly List<EntityPosition> SpawnListPlane = new List<EntityPosition>
        {
            new EntityPosition(new Vector3(1638.067f, 3234.868f, 40.11113f), 103.8905f),
            new EntityPosition(new Vector3(1558.921f, 3155.603f, 40.23004f), 134.3105f),
            new EntityPosition(new Vector3(1430.566f, 3111.669f, 40.23326f), 103.7299f),
            new EntityPosition(new Vector3(2071.546f, 4786.328f, 40.79108f), 115.6482f)
        };

        private static readonly List<EntityPosition> SpawnListBoat = new List<EntityPosition>
        {
            new EntityPosition(new Vector3(-989.812f, -1395.955f, 0.3117422f), 197.2292f),
            new EntityPosition(new Vector3(-998.601f, -1400.204f, -0.01398028f), 200.5666f),
            new EntityPosition(new Vector3(-982.6636f, -1392.835f, -0.1012118f), 200.7435f),
            new EntityPosition(new Vector3(-973.8835f, -1389.073f, -0.07558359f), 196.9216f),
            new EntityPosition(new Vector3(-965.5593f, -1385.88f, 0.1165133f), 197.4063f),
            new EntityPosition(new Vector3(-955.8226f, -1383.237f, -0.06844078f), 199.7236f),
            new EntityPosition(new Vector3(-930.0321f, -1374.57f, 0.024976f), 196.3813f),
            new EntityPosition(new Vector3(-911.7566f, -1368.049f, 0.01486713f), 205.7693f),
            new EntityPosition(new Vector3(-845.9328f, -1362.563f, -0.1105901f), 287.3574f),
            new EntityPosition(new Vector3(-858.0338f, -1328.371f, -0.05082685f), 290.1963f),
            new EntityPosition(new Vector3(-897.3489f, -1336.532f, 0.0469994f), 115.3039f),
            new EntityPosition(new Vector3(-915.5237f, -1343.745f, 0.1156863f), 111.007f),
            new EntityPosition(new Vector3(-948.0477f, -1355.498f, 0.124268f), 106.5388f),
            new EntityPosition(new Vector3(-836.3871f, -1389.433f, 0.03498344f), 290.8258f),
            new EntityPosition(new Vector3(-785.4754f, -1440.365f, 0.07404654f), 322.9755f),
            new EntityPosition(new Vector3(-772.8375f, -1424.96f, 0.07902782f), 317.0082f),
            new EntityPosition(new Vector3(-774.7501f, -1385.55f, 0.09457491f), 50.25464f),
            new EntityPosition(new Vector3(-753.9382f, -1362.394f, -0.04268733f), 50.26961f),
            new EntityPosition(new Vector3(-724.7479f, -1327.435f, 0.02641343f), 52.78964f),
            new EntityPosition(new Vector3(-855.8542f, -1485.853f, 0.0313217f), 108.474f)
        };

        private static readonly List<EntityPosition> SpawnListMilitary = new List<EntityPosition>
        {
            new EntityPosition(new Vector3(-1594.426f, 3185.479f, 30.40495f), 147.6925f),
            new EntityPosition(new Vector3(-1603.479f, 3203.978f, 30.41406f), 171.5964f),
            new EntityPosition(new Vector3(-1615.621f, 3169.568f, 29.66991f), 223.9812f),
            new EntityPosition(new Vector3(-1580.501f, 3156.202f, 30.64534f), 154.0106f),
            new EntityPosition(new Vector3(-1565.606f, 3131.228f, 32.23048f), 142.0033f),
            new EntityPosition(new Vector3(-1630.897f, 2980.762f, 32.45866f), 251.8481f),
            new EntityPosition(new Vector3(-1565.328f, 3020.8f, 32.43408f), 121.0561f),
            new EntityPosition(new Vector3(-1668.656f, 3081.12f, 30.85717f), 231.5131f)
        };

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static EntityPosition GetRecoverNode(Vehicle veh)
        {
            List<EntityPosition> tempList = new List<EntityPosition>();

            if (veh.Model.IsHelicopter || veh.Model.IsCargobob)
                tempList.AddRange(SpawnListHeli);
            else if (veh.Model.IsPlane)
                tempList.AddRange(SpawnListPlane);
            else if (veh.Model.IsBoat)
                tempList.AddRange(SpawnListBoat);
            else if (veh.ClassType == VehicleClass.Military)
                tempList.AddRange(SpawnListMilitary);
            else
            {
                veh.Model.GetDimensions(out Vector3 min, out Vector3 max);
                Vector3 vehDimension = max - min;
                if (vehDimension.Y > 7.4f)
                    tempList.AddRange(SpawnListVehicleLong);
                else
                    tempList.AddRange(SpawnListVehicle);
            }

            Random rnd = new Random();
            while (tempList.Count > 0)
            {
                int n = rnd.Next(0, tempList.Count);
                EntityPosition spawn = tempList[n];

                if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, spawn.Position.X, spawn.Position.Y, spawn.Position.Z, 
                    5.0f, 5.0f, 5.0f, 0))
                    return spawn;
                else
                    tempList.Remove(spawn);
            }

            // Si no hay ningún punto libre, limpiamos uno aleatorio de coches
            EntityPosition clearSpawn = SpawnListVehicle[rnd.Next(0, SpawnListVehicle.Count)];
            Function.Call(Hash.CLEAR_AREA_OF_VEHICLES, clearSpawn.Position.X, clearSpawn.Position.Y, clearSpawn.Position.Z, 1.0f, 
                false, false, false, false, false);
            return clearSpawn;
        }
    }
}