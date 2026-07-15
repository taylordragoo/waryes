using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_UnitSpawner : MonoBehaviour
    {
        public AvailableUnit[] availableUnits;
        public Transform spawnPont;
        public Transform asamblePoint;
        public bool canBuild = true;

        [System.Serializable]
        public class AvailableUnit
        {
            public string name;
            public GameObject unitPrefab;
            public GameObject uiButton;
            public int cost;
        }

        public void SpawnUnitByIndex (int index)
        {
            if(GetComponent<uSimRTS_BaseBuilding>() != null)
                if (GetComponent<uSimRTS_BaseBuilding>().playerCommander.GetComponent<uSimRTS_EconomyManager>() != null)
                    if (GetComponent<uSimRTS_BaseBuilding>().playerCommander.GetComponent<uSimRTS_EconomyManager>().currentCredits >= availableUnits[index].cost)
                        if (canBuild)
                            StartCoroutine(WaitForBuildTimeAndSpawn(availableUnits[index].unitPrefab.GetComponent<uSimRTS_Unit>().buildTime, index));
        }

        IEnumerator WaitForBuildTimeAndSpawn (float time, int index)
        {
            canBuild = false;

            GetComponent<uSimRTS_BaseBuilding>().playerCommander.GetComponent<uSimRTS_EconomyManager>().SubstractCredits(availableUnits[index].cost);

            yield return new WaitForSeconds(time);

            GameObject newUnit = Instantiate(availableUnits[index].unitPrefab, spawnPont.position, spawnPont.rotation);
            uSimRTS_Unit unit = newUnit.GetComponent<uSimRTS_Unit>();

            if (unit.GetComponent<uSimRTS_Unit>().useNavMeshAgent)
                unit.GetComponent<uSimRTS_Unit>().navMeshAgent.SetDestination(asamblePoint.position);
            else
                 unit.waypoint.position = asamblePoint.position;

          
            
            if(GetComponent<uSimRTS_BaseBuilding>().side == uSimRTS_Manager.instance.playerSide)
                GameObject.FindObjectOfType<uSimRTS_UnitsCommand>().playerUnits.Add(unit.GetComponent<uSimRTS_Unit>());
            else
            {
                GetComponent<uSimRTS_BaseBuilding>().commander.currentUnits.Add(unit.GetComponent<uSimRTS_Unit>());
                unit.GetComponent<uSimRTS_UnitAI>().commander = GetComponent<uSimRTS_BaseBuilding>().commander;
            }

            canBuild = true;
        }
    }
}
