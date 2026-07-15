using UnityEngine;
using UnityEngine.AI;

namespace uSimRTS
{
    public static class uSimRTS_ProjectScale
    {
        public const float WorldScale = 20f;

        // NavMeshAgent radius and height inherit transform scale, so keep these
        // in local units while the baked NavMesh uses their world-size values.
        private const float AgentRadius = 0.4f / WorldScale;
        private const float AgentHeight = 1.8f / WorldScale;
        private const float AgentSpeed = 4f;
        private const float AgentAcceleration = 8f;
        private const float AgentStoppingDistance = 0.5f;

        public static void ApplyToGameplayRoot(Component component)
        {
            Transform gameplayRoot = component.transform;

            for (Transform candidate = component.transform; candidate != null; candidate = candidate.parent)
            {
                if (candidate.GetComponent<uSimRTS_Unit>() != null ||
                    candidate.GetComponent<uSimRTS_BaseBuilding>() != null)
                {
                    gameplayRoot = candidate;
                }
            }

            gameplayRoot.localScale = Vector3.one * WorldScale;
        }

        public static void ConfigureUnit(uSimRTS_Unit unit)
        {
            ApplyToGameplayRoot(unit);

            if (unit.useNavMeshAgent)
            {
                NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
                unit.navMeshAgent = agent;

                if (agent == null)
                    return;

                agent.radius = AgentRadius;
                agent.height = AgentHeight;
                agent.speed = AgentSpeed;
                agent.acceleration = AgentAcceleration;
                agent.stoppingDistance = AgentStoppingDistance;
                agent.autoBraking = true;
                return;
            }

            unit.maxSpeed *= WorldScale;
            unit.accel *= WorldScale;
            unit.size *= WorldScale;
        }

        public static void ConfigureRanges(uSimRTS_Unit unit)
        {
            uSimRTS_UnitRadar radar = unit.GetComponent<uSimRTS_UnitRadar>();

            if (radar != null)
            {
                radar.range *= WorldScale;
                radar.visRange *= WorldScale;
            }

            uSimRTS_UnitFowAgent fogAgent = unit.GetComponent<uSimRTS_UnitFowAgent>();

            if (fogAgent != null)
                fogAgent.radius *= WorldScale;
        }
    }
}
