using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_CommandAI : MonoBehaviour
    {
        [Tooltip("Auto assigned")]
        public uSimRTS_Manager.Sides side;
        [Tooltip("Limits the amount of units the AI can have simultaniusly. If it has enough credits.")]
        public int maxUnits = 12;

        [Tooltip("Buildings in an enemy side.")]
        public List<uSimRTS_BaseBuilding> enemyBaseBuildings;
        [Tooltip("Spawned units.")]
        public List<uSimRTS_Unit> currentUnits;
        [Tooltip("Own buildings.")]
        public List<uSimRTS_BaseBuilding> currentBaseBuildings;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            //function that spawns the units. By code it spawns the first unit on the list of each building.
            foreach (uSimRTS_BaseBuilding building in currentBaseBuildings)
                if(building.spawner != null && currentUnits.Count < maxUnits && building.spawner.availableUnits.Length > 0)
                    building.spawner.SpawnUnitByIndex(0); // <-- implement custom index if want to spawn different units on the same building.

            //AI attacks once every 45 seconds.
            if (!attacking)
                StartCoroutine(WaitAndAttackEnemyBase());

        }

        bool attacking;
        IEnumerator WaitAndAttackEnemyBase ()
        {
            attacking = true;
            
            GetEnemyBuildings();

            if (enemyBaseBuildings.Count > 0)
            {
                try
                {
                    foreach (uSimRTS_Unit unit in currentUnits)
                    {
                        Vector3 pos = enemyBaseBuildings[0].transform.position - ((enemyBaseBuildings[0].transform.position - unit.transform.position).normalized * unit.radar.range * 0.85f);
                        pos.y = 0f;
                        if (!unit.useNavMeshAgent)
                            unit.waypoint.position = pos;
                        else
                            unit.navMeshAgent.SetDestination(pos);

                        unit.waypointOverlapChecker.CheckWaypointOverlaping();
                    }
                }
                catch
                {

                }
            }

            yield return new WaitForSeconds(45); //<-- implement custom cool off timer here.

            attacking = false;
        }
       

        uSimRTS_Unit ClosestUnitToPos(Vector3 pos)
        {
            uSimRTS_Unit closestUnit = null;
            float closestDist = 100;

            for (int i = 0; i < currentUnits.Count; i++)
            {
                uSimRTS_Unit unit = currentUnits[i];
                float dist = Vector3.Distance(unit.transform.position, pos);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestUnit = unit;
                }
            }

            return closestUnit;
        }

        public void RemoveFromUnitsList(uSimRTS_Unit tgtUnit)
        {
            try
            {
                foreach (uSimRTS_Unit unit in currentUnits)
                    if (unit == tgtUnit)
                        currentUnits.Remove(unit);
            }
            catch
            {


            }
        }

        void GetEnemyBuildings ()
        {
            uSimRTS_BaseBuilding[] buildings = GameObject.FindObjectsOfType<uSimRTS_BaseBuilding>();

            enemyBaseBuildings.Clear();

            foreach (uSimRTS_BaseBuilding building in buildings)
            {
                if (building.side != side)
                    enemyBaseBuildings.Add(building);
            }
        }
    }
}
