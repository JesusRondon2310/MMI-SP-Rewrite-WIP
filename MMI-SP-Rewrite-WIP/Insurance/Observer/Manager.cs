using GTA;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using MMI_SP.PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Insurance.Observer
{
    public class Manager : Script
    {
        // ==========================================
        // BLOQUE: Datos
        // ==========================================
        private static Manager _instance;
        internal static Manager Instance => _instance;

        private static bool _initialized = false;
        internal static bool Initialized => _initialized;

        private static List<Vehicle> _insuredVehList = new List<Vehicle>();
        internal static List<Vehicle> InsuredVehList => _insuredVehList;

        private static List<Vehicle> _recoveredVehList = new List<Vehicle>();
        internal static List<Vehicle> RecoveredVehList => _recoveredVehList;

        private static Dictionary<string, Blip> _blipsToRemove = new Dictionary<string, Blip>();
        internal static Dictionary<string, Blip> BlipsToRemove => _blipsToRemove;

        private int _timerInsurance = 0;
        private int _timerDetectInsuredVehicles = 0;
        private int _timerRecoveredVehicle = 0;
        private int _delayDetectInsuredVehicles = 3000;

        // ==========================================
        // BLOQUE: Constructor y eventos del Script
        // ==========================================
        public Manager()
        {
            _instance = this;
            Tick += Initialize;
        }

        private void Initialize(object sender, EventArgs e)
        {
            while (!MMI.IsInitialized) Yield();

            _initialized = true;

            RestoreVehiclesFromDatabase();

            Tick -= Initialize;
            Aborted += OnAborted;
            Tick += OnTick;
        }

        private void RestoreVehiclesFromDatabase()
        {
            List<VehicleData> allVehicles = Insurer.Instance.GetInsuredListFull();
            if (allVehicles == null || allVehicles.Count == 0) return;

            foreach (VehicleData data in allVehicles)
            {
                if (data.IsDestroyed) continue;

                // Verificar si ya existe en el mundo (por ID)
                string vehId = data.Id;
                bool alreadyExists = World.GetAllVehicles().Any(v => VehicleIdentifier.Get(v) == vehId);
                if (alreadyExists) continue;

                var result = VehiclePersistence.SpawnVehicle(data);
                if (result is Ok<Vehicle> ok)
                {
                    Vehicle spawned = ok.Value;
                    _insuredVehList.Add(spawned);

                    var blipResult = BlipManager.AddVehicleBlip(spawned);
                    if (blipResult is Ok<Blip> okBlip)
                        _blipsToRemove[vehId] = okBlip.Value;
                }
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Insurer.Instance == null) return;

            ProcessTimers();
            VehicleChangeHandler.Handle(_insuredVehList, _recoveredVehList, _blipsToRemove);
        }

        private void ProcessTimers()
        {
            if (_timerInsurance <= Game.GameTime)
            {
                VehicleMonitor.UpdateInsurance(Insurer.Instance, _insuredVehList, _blipsToRemove);
                _timerInsurance = Game.GameTime + 1000;
            }
            if (_timerRecoveredVehicle <= Game.GameTime)
            {
                RecoveryManager.UpdateRecoveredVehicles(_recoveredVehList);
                _timerRecoveredVehicle = Game.GameTime + 3000;
            }
            if (_timerDetectInsuredVehicles <= Game.GameTime)
            {
                VehicleMonitor.CheckForInsuredVehicles(Insurer.Instance, _insuredVehList, _blipsToRemove);
                _timerDetectInsuredVehicles = Game.GameTime + _delayDetectInsuredVehicles;
            }
        }

        private void OnAborted(object sender, EventArgs e)
        {
            BlipManager.ClearAllBlips(_blipsToRemove);
            PersistenceManager.RemovePersistence(_recoveredVehList);
        }
    }
}