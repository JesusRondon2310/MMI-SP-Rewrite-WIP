using GTA;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMI_SP.Insurance.Observer
{
    public class Manager : Script
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private static Manager _instance;
        private static List<Vehicle> _recoveredVehList = new List<Vehicle>();
        private static Dictionary<string, Blip> _blipsToRemove = new Dictionary<string, Blip>();
        private static bool _initialized = false;
        private static List<Vehicle> _insuredVehList = new List<Vehicle>();
        internal static Manager Instance => _instance;
        internal static bool Initialized => _initialized;
        internal static List<Vehicle> InsuredVehList => _insuredVehList;
        internal static List<Vehicle> RecoveredVehList => _recoveredVehList;
        internal static Dictionary<string, Blip> BlipsToRemove => _blipsToRemove;

        private int _timerInsurance = 0;
        private int _timerDetectInsuredVehicles = 0;
        private int _timerRecoveredVehicle = 0;
        private int _delayDetectInsuredVehicles = 3000;

        // ==========================================
        // BLOQUE 2: Constructor y eventos del Script
        // ==========================================
        public Manager()
        {
            _instance = this;
            Tick += Initialize;
            KeyUp += OnKeyUp;
        }

        private void Initialize(object sender, EventArgs e)
        {
            while (!MMI.IsInitialized) Yield();

            _initialized = true;

            ObserverInitializer.RestoreVehiclesFromDatabase(_insuredVehList, _blipsToRemove);

            Tick -= Initialize;
            Aborted += OnAborted;
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Insurer.Instance == null) return;

            ProcessTimers();
            VehicleChangeHandler.Handle(_insuredVehList, _recoveredVehList, _blipsToRemove);

            LockVehicle.ShowHint(_insuredVehList);
            LockVehicle.UpdateSequence();
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

        public static void RemoveVehicleFromObservation(string vehicleId)
        {
            Vehicle veh = InsuredVehList.FirstOrDefault(v => VehicleIdentifier.Get(v) == vehicleId);
            if (veh != null)
            {
                BlipManager.RemoveRecoverBlip(veh, BlipsToRemove);
                InsuredVehList.Remove(veh);
            }
        }

        private void OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.L)
                LockVehicle.Toggle(_insuredVehList, Insurer.Instance);
        }
    }
}