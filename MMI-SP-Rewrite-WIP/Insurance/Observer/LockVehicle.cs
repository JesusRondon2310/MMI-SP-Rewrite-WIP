using GTA;
using MMI_SP.Helpers;
using MMI_SP.Insurance.Policies;
using System.Collections.Generic;

namespace MMI_SP.Insurance.Observer
{
    internal static class LockVehicle
    {
        private const float MaxDistance = 2.5f;

        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal static bool SequenceRunning => LockFeedback.Running;
        internal static int SequenceStep => LockFeedback.Step;
        internal static int NextStepTime => LockFeedback.NextStepTime;
        internal static Vehicle SequenceVeh => LockFeedback.SequenceVeh;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void ShowHint(List<Vehicle> insuredVehList)
        {
            if (LockFeedback.Running) return;

            Ped player = Game.Player.Character;
            if (player == null || player.IsInVehicle()) return;

            Vehicle nearest = FindNearestInsuredVehicle(player, insuredVehList);
            if (nearest != null)
            {
                string action = nearest.LockStatus == VehicleLockStatus.CannotEnter
                    ? "Desbloquear"
                    : "Bloquear";
                GTA.UI.Screen.ShowHelpTextThisFrame($"Presiona ~y~L~w~ para {action} el vehículo");
            }
        }

        internal static void Toggle(List<Vehicle> insuredVehList, Insurer insurer)
        {
            if (LockFeedback.Running) return;

            Ped player = Game.Player.Character;
            if (player == null || player.IsInVehicle()) return;

            Vehicle target = FindNearestInsuredVehicle(player, insuredVehList);
            if (target == null) return;

            bool willBeLocked = target.LockStatus != VehicleLockStatus.CannotEnter;
            target.LockStatus = willBeLocked ? VehicleLockStatus.CannotEnter : VehicleLockStatus.Unlocked;

            if (willBeLocked) target.IsAlarmSet = true;

            string vehId = VehicleIdentifier.Get(target);
            LockPersistenceHelper.Persist(insurer, vehId, willBeLocked);

            LockFeedback.Start(target);
        }

        internal static void UpdateSequence()
        {
            LockFeedback.Update();
        }

        private static Vehicle FindNearestInsuredVehicle(Ped player, List<Vehicle> insuredVehList)
        {
            Vehicle nearest = null;
            float minDist = MaxDistance;

            foreach (Vehicle veh in insuredVehList)
            {
                if (veh == null || !veh.Exists() || veh.IsDead) continue;
                float dist = player.Position.DistanceTo(veh.Position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = veh;
                }
            }
            return nearest;
        }
    }
}