using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

            if (unit.useNavMeshAgent)
            {
                NavMeshAgent agent = unit.navMeshAgent != null
                    ? unit.navMeshAgent
                    : unit.GetComponent<NavMeshAgent>();

                NavMeshHit destinationHit = new NavMeshHit();
                bool hasDestination = agent != null &&
                    NavMesh.SamplePosition(asamblePoint.position, out destinationHit, 10f, agent.areaMask);
                NavMeshHit spawnHit = new NavMeshHit();
                NavMeshPath spawnPath = new NavMeshPath();
                bool hasSpawnRoute = hasDestination && TryFindSpawnRoute(
                    spawnPont.position,
                    destinationHit.position,
                    agent.areaMask,
                    out spawnHit,
                    out spawnPath);

                if (hasSpawnRoute && agent.Warp(spawnHit.position))
                    agent.SetPath(spawnPath);
                else
                    Debug.LogWarning($"Could not find a complete NavMesh route for spawned unit '{newUnit.name}'.", newUnit);
            }
            else
                unit.SetDestination(asamblePoint.position);

          
            
            if(GetComponent<uSimRTS_BaseBuilding>().side == uSimRTS_Manager.instance.playerSide)
                GameObject.FindObjectOfType<uSimRTS_UnitsCommand>().playerUnits.Add(unit.GetComponent<uSimRTS_Unit>());
            else
            {
                GetComponent<uSimRTS_BaseBuilding>().commander.currentUnits.Add(unit.GetComponent<uSimRTS_Unit>());
                unit.GetComponent<uSimRTS_UnitAI>().commander = GetComponent<uSimRTS_BaseBuilding>().commander;
            }

            canBuild = true;
        }

        private static bool TryFindSpawnRoute(
            Vector3 desiredSpawnPosition,
            Vector3 destination,
            int areaMask,
            out NavMeshHit spawnHit,
            out NavMeshPath path)
        {
            const float ringSpacing = 2.5f;
            const float sampleRadius = 2f;
            const int ringCount = 12;
            const int samplesPerRing = 24;

            spawnHit = new NavMeshHit();
            path = new NavMeshPath();

            for (int ring = 0; ring <= ringCount; ring++)
            {
                float radius = ring * ringSpacing;
                int sampleCount = ring == 0 ? 1 : samplesPerRing;

                for (int sample = 0; sample < sampleCount; sample++)
                {
                    float angle = sample * Mathf.PI * 2f / sampleCount;
                    Vector3 candidate = desiredSpawnPosition + new Vector3(
                        Mathf.Cos(angle) * radius,
                        0f,
                        Mathf.Sin(angle) * radius);

                    NavMeshHit candidateHit;
                    if (!NavMesh.SamplePosition(candidate, out candidateHit, sampleRadius, areaMask))
                        continue;

                    NavMeshPath candidatePath = new NavMeshPath();
                    if (!NavMesh.CalculatePath(candidateHit.position, destination, areaMask, candidatePath))
                        continue;

                    if (candidatePath.status != NavMeshPathStatus.PathComplete)
                        continue;

                    spawnHit = candidateHit;
                    path = candidatePath;
                    return true;
                }
            }

            return false;
        }
    }
}
