using UnityEngine;
using UnityEngine.AI;

namespace uSimRTS
{
    /// <summary>
    /// Small, stable surface exposed to the OneJS HUD. It deliberately adapts the
    /// existing RTS scripts instead of making the frontend depend on their fields.
    /// </summary>
    public sealed class uSimRTS_HudBridge : MonoBehaviour
    {
        [SerializeField] private uSimRTS_Manager manager;
        [SerializeField] private uSimRTS_UnitsCommand unitsCommand;

        private float nextReferenceRefresh;
        private string activeRule = "FIRE AT WILL";
        private string lastOrder = "AWAITING ORDERS";

        public int Credits
        {
            get
            {
                if (manager == null || manager.player == null)
                    return 0;

                uSimRTS_EconomyManager economy = manager.player.GetComponent<uSimRTS_EconomyManager>();
                return economy != null ? economy.currentCredits : 0;
            }
        }

        public int PlayerUnitCount => unitsCommand != null && unitsCommand.playerUnits != null
            ? unitsCommand.playerUnits.Count
            : 0;

        public int SelectedUnitCount => unitsCommand != null && unitsCommand.selectedUnits != null
            ? unitsCommand.selectedUnits.Count
            : 0;

        public string PlayerSide => manager != null ? manager.playerSide.ToString().ToUpperInvariant() : "BLUE";

        public string SelectedUnitName
        {
            get
            {
                uSimRTS_Unit unit = FirstSelectedUnit;
                return unit != null ? unit.gameObject.name.ToUpperInvariant() : "NO UNIT SELECTED";
            }
        }

        public string SelectedUnitStatus
        {
            get
            {
                uSimRTS_Unit unit = FirstSelectedUnit;
                if (unit == null)
                    return "STANDBY";

                if (unit.combatController != null && unit.combatController.firing)
                    return "ENGAGING";

                if (unit.useNavMeshAgent && unit.navMeshAgent != null && unit.navMeshAgent.isOnNavMesh &&
                    unit.navMeshAgent.hasPath)
                    return "MOVING";

                return "READY";
            }
        }

        public string WeaponName
        {
            get
            {
                uSimRTS_Unit unit = FirstSelectedUnit;
                if (unit == null || unit.combatController == null || unit.combatController.weapons == null ||
                    unit.combatController.weapons.Length == 0)
                    return "NO WEAPON DATA";

                string weaponName = unit.combatController.weapons[0].name;
                return string.IsNullOrWhiteSpace(weaponName) ? "PRIMARY WEAPON" : weaponName.ToUpperInvariant();
            }
        }

        public int ElapsedSeconds => Mathf.FloorToInt(Time.timeSinceLevelLoad);
        public float TimeScale => Time.timeScale;
        public bool IsPaused => Mathf.Approximately(Time.timeScale, 0f);
        public string ActiveRule => activeRule;
        public string LastOrder => lastOrder;

        private uSimRTS_Unit FirstSelectedUnit
        {
            get
            {
                if (unitsCommand == null || unitsCommand.selectedUnits == null || unitsCommand.selectedUnits.Count == 0)
                    return null;

                return unitsCommand.selectedUnits[0];
            }
        }

        private void Awake()
        {
            RefreshReferences();
        }

        private void Update()
        {
            if (Time.unscaledTime >= nextReferenceRefresh && (manager == null || unitsCommand == null))
                RefreshReferences();
        }

        public void SetTimeScale(float value)
        {
            Time.timeScale = Mathf.Clamp(value, 0f, 4f);
        }

        public void SetEngagementRule(string rule)
        {
            if (string.IsNullOrWhiteSpace(rule))
                return;

            activeRule = rule.ToUpperInvariant();
            lastOrder = activeRule;
        }

        public void IssueOrder(string order)
        {
            if (string.IsNullOrWhiteSpace(order))
                return;

            lastOrder = order.ToUpperInvariant();
            if (string.Equals(order, "Stop", System.StringComparison.OrdinalIgnoreCase))
                StopSelectedUnits();
            else if (string.Equals(order, "Clear Selection", System.StringComparison.OrdinalIgnoreCase))
                ClearSelection();
        }

        public void StopSelectedUnits()
        {
            if (unitsCommand == null || unitsCommand.selectedUnits == null)
                return;

            foreach (uSimRTS_Unit unit in unitsCommand.selectedUnits)
            {
                if (unit == null)
                    continue;

                if (unit.useNavMeshAgent && unit.navMeshAgent != null && unit.navMeshAgent.isOnNavMesh)
                    unit.navMeshAgent.ResetPath();
                else if (unit.waypoint != null)
                    unit.SetDestination(unit.transform.position);
            }

            lastOrder = "STOP";
        }

        public void ClearSelection()
        {
            if (unitsCommand != null)
                unitsCommand.SetSelectionEmpty();

            lastOrder = "SELECTION CLEARED";
        }

        private void RefreshReferences()
        {
            if (manager == null)
                manager = FindFirstObjectByType<uSimRTS_Manager>();

            if (unitsCommand == null)
                unitsCommand = FindFirstObjectByType<uSimRTS_UnitsCommand>();

            nextReferenceRefresh = Time.unscaledTime + 1f;
        }
    }
}
