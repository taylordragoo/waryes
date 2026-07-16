using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace uSimRTS
{
    public class uSimRTS_Unit : MonoBehaviour
    {
        [Tooltip("Current waypoint (internal use)")]
        public Transform waypoint;
        [Tooltip("Overlap check script reference (internal use)")]
        public uSimRTS_WaypointOverlapChecker waypointOverlapChecker;
        [Tooltip("Max speed of the unit")]
        public float maxSpeed;
        [Tooltip("Max speed of the unit / adjust for scale")]
        public float speed = 0f;
        [Tooltip("acceleration of unit / adjust for scale")]
        public float accel = 1f;
        [Tooltip("max turn speed when stoped")]
        public float turnSpeed = 1f;
        [Tooltip("Size of the checker bounds")]
        public float size = 0.1f;
        [Tooltip("Use to adjust build time")]
        public float buildTime;
        [Tooltip("Use unity nav agent or uSim movement")]
        public bool useNavMeshAgent;
        public NavMeshAgent navMeshAgent;

        [Header("Road-Preferred Navigation")]
        [Tooltip("Use a cost-filtered NavMesh path as the steering target while retaining uSim vehicle movement.")]
        public bool preferRoads = true;
        [Tooltip("Per-query traversal cost for the Road NavMesh area.")]
        [Min(1f)] public float roadPathCost = 1f;
        [Tooltip("Per-query traversal cost for the default Walkable area.")]
        [Min(1f)] public float terrainPathCost = 1.75f;
        [Tooltip("Maximum distance used to project the vehicle and destination onto the NavMesh.")]
        [Min(0.1f)] public float roadPathSampleDistance = 8f;
        [Tooltip("Minimum distance at which a path corner is considered reached.")]
        [Min(0.1f)] public float roadPathCornerDistance = 0.75f;
        [Tooltip("Minimum delay between path calculations when a destination moves repeatedly.")]
        [Min(0f)] public float roadPathRecalculationDelay = 0.25f;
        [Tooltip("NavMesh agent type used for vehicle path queries. Zero is the project's Humanoid bake.")]
        public int roadPathAgentTypeId;

        [Tooltip("internal use")]
        public Transform pointer;
        [Tooltip("internal use")]
        public GameObject selector;
        [Tooltip("internal use")]
        public uSimRTS_CombatController combatController;
        [Tooltip("internal use")]
        public uSimRTS_UnitRadar radar;

        [Tooltip("Movement input. internal use")]
        public float moveInput;
        [Tooltip("internal use")]
        public float distToTgt;
        [Tooltip("internal use")]
        public float speedCoef = 1f;
        [Tooltip("internal use")]
        public Vector2 inverseTgtPos;
        [Tooltip("internal use")]
        public GameObject blockingObject;
        Vector3 lastHitPos;
        Vector3 dirToWp;

        const string RoadAreaName = "Road";
        const string TerrainAreaName = "Walkable";
        const float DestinationChangeThreshold = 0.05f;

        NavMeshPath roadPath;
        Vector3[] roadPathCorners = System.Array.Empty<Vector3>();
        int roadPathCornerIndex;
        Vector3 evaluatedRoadPathDestination;
        bool hasEvaluatedRoadPathDestination;
        float nextRoadPathCalculationTime;

        const float GroundProbeHeight = 100f;
        const float GroundProbeDistance = 250f;
        readonly RaycastHit[] groundHits = new RaycastHit[32];
        readonly RaycastHit[] obstacleHits = new RaycastHit[32];

        void Awake()
        {
            uSimRTS_ProjectScale.ConfigureUnit(this);
            uSimRTS_ProjectScale.ConfigureRanges(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            waypoint.gameObject.hideFlags = HideFlags.HideInHierarchy;
            waypoint.parent = null;
            waypointOverlapChecker = waypoint.GetComponent<uSimRTS_WaypointOverlapChecker>();
            waypointOverlapChecker.ownerUnit = this;

            if (selector == null)
               selector = transform.Find("selector").gameObject;
            if (combatController == null)
                combatController = GetComponent<uSimRTS_CombatController>();
            radar = GetComponent<uSimRTS_UnitRadar>();

            if (useNavMeshAgent)
                navMeshAgent = GetComponent<NavMeshAgent>();
            else
            {
                roadPath = new NavMeshPath();
                SnapToGround(transform.position);
            }

            waypointOverlapChecker.CheckWaypointOverlaping();

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!useNavMeshAgent) 
            {
                Vector3 movementTarget = GetCustomMovementTarget();
            
                if(moveInput > 0f)
                CheckClearPath(dirToWp);


              //  pointer.rotation = Quaternion.Lerp(pointer.rotation,, Time.deltaTime);
                Vector3 waypointDirection = PlanarDirectionTo(movementTarget);
                if (waypointDirection.sqrMagnitude > 0.0001f)
                    pointer.rotation = Quaternion.LookRotation(waypointDirection, Vector3.up);


                if (!blockingObject)
                    dirToWp = PlanarDirectionTo(movementTarget);



                Vector3 planarWaypoint = movementTarget;
                planarWaypoint.y = transform.position.y;
                inverseTgtPos = transform.InverseTransformPoint(planarWaypoint);



                if (blockingObject != null && moveInput > 0f)
                {
                    Vector3 inversePos = pointer.InverseTransformPoint(blockingObject.transform.position);
                    if (inversePos.z > 0f)
                    {
                        if (inversePos.x <= 0f)
                            dirToWp = PlanarDirectionTo(lastHitPos + (pointer.right * size * 1.2f));
                        if (inversePos.x > 0f)
                            dirToWp = PlanarDirectionTo(lastHitPos - (pointer.right * size * 1.2f));
                    }
                }


                if (dirToWp.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dirToWp, Vector3.up), Time.deltaTime * turnSpeed);
           


                    moveInput = 1f;
                distToTgt = PlanarDirectionTo(movementTarget).magnitude;
                if (distToTgt < size || colliding)
                    moveInput = -3f;


                speedCoef = 1f;

                if (Mathf.Abs(inverseTgtPos.x) > 0.5f)
                    speedCoef = 0.2f;



                float curMaxSpeed = maxSpeed * speedCoef;
                speed += moveInput * (accel / 100f) * Time.deltaTime;
                speed = Mathf.Clamp(speed, 0f, curMaxSpeed / 100f);

                SnapToGround(transform.position + (transform.forward * speed));
            }
            else {

                Vector3 lookDirection = Vector3.zero;

                if (combatController && combatController.firing && combatController.target)
                    lookDirection = PlanarDirectionTo(combatController.target.position);
                else if (navMeshAgent != null && navMeshAgent.isOnNavMesh && navMeshAgent.hasPath)
                    lookDirection = PlanarDirectionTo(navMeshAgent.steeringTarget);

                if (lookDirection.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            }
        }

        Vector3 PlanarDirectionTo(Vector3 worldPosition)
        {
            return Vector3.ProjectOnPlane(worldPosition - transform.position, Vector3.up);
        }

        public void SetDestination(Vector3 destination)
        {
            if (useNavMeshAgent)
            {
                if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
                    navMeshAgent.SetDestination(destination);

                return;
            }

            if (waypoint == null)
                return;

            if ((waypoint.position - destination).sqrMagnitude > DestinationChangeThreshold * DestinationChangeThreshold)
                hasEvaluatedRoadPathDestination = false;

            waypoint.position = destination;
        }

        Vector3 GetCustomMovementTarget()
        {
            Vector3 destination = waypoint.position;

            if (!preferRoads)
            {
                ClearRoadPath();
                return destination;
            }

            bool destinationChanged = !hasEvaluatedRoadPathDestination ||
                (evaluatedRoadPathDestination - destination).sqrMagnitude > DestinationChangeThreshold * DestinationChangeThreshold;

            if (destinationChanged)
            {
                ClearRoadPath();

                if (Time.time >= nextRoadPathCalculationTime)
                {
                    CalculateRoadPreferredPath(destination);
                    evaluatedRoadPathDestination = destination;
                    hasEvaluatedRoadPathDestination = true;
                    nextRoadPathCalculationTime = Time.time + roadPathRecalculationDelay;
                }
            }

            float cornerDistance = Mathf.Max(roadPathCornerDistance, size);
            float cornerDistanceSquared = cornerDistance * cornerDistance;

            while (roadPathCornerIndex < roadPathCorners.Length &&
                PlanarDirectionTo(roadPathCorners[roadPathCornerIndex]).sqrMagnitude <= cornerDistanceSquared)
            {
                roadPathCornerIndex++;
            }

            if (roadPathCornerIndex < roadPathCorners.Length)
                return roadPathCorners[roadPathCornerIndex];

            ClearRoadPath();
            return destination;
        }

        void CalculateRoadPreferredPath(Vector3 destination)
        {
            int roadArea = NavMesh.GetAreaFromName(RoadAreaName);
            int terrainArea = NavMesh.GetAreaFromName(TerrainAreaName);

            if (roadArea < 0 || terrainArea < 0)
                return;

            NavMeshQueryFilter filter = new NavMeshQueryFilter
            {
                agentTypeID = roadPathAgentTypeId,
                areaMask = NavMesh.AllAreas
            };

            filter.SetAreaCost(roadArea, Mathf.Max(1f, roadPathCost));
            filter.SetAreaCost(terrainArea, Mathf.Max(1f, terrainPathCost));

            if (!NavMesh.SamplePosition(transform.position, out NavMeshHit startHit, roadPathSampleDistance, filter) ||
                !NavMesh.SamplePosition(destination, out NavMeshHit destinationHit, roadPathSampleDistance, filter))
            {
                return;
            }

            if (roadPath == null)
                roadPath = new NavMeshPath();

            roadPath.ClearCorners();

            if (!NavMesh.CalculatePath(startHit.position, destinationHit.position, filter, roadPath) ||
                roadPath.status != NavMeshPathStatus.PathComplete)
            {
                return;
            }

            Vector3[] corners = roadPath.corners;
            if (corners == null || corners.Length == 0)
                return;

            roadPathCorners = corners;
            roadPathCornerIndex = 0;
        }

        void ClearRoadPath()
        {
            roadPathCorners = System.Array.Empty<Vector3>();
            roadPathCornerIndex = 0;
        }

        void OnDrawGizmosSelected()
        {
            if (roadPathCorners == null || roadPathCorners.Length == 0)
                return;

            Gizmos.color = Color.cyan;
            Vector3 previous = transform.position;

            for (int i = roadPathCornerIndex; i < roadPathCorners.Length; i++)
            {
                Gizmos.DrawLine(previous, roadPathCorners[i]);
                Gizmos.DrawSphere(roadPathCorners[i], 0.25f);
                previous = roadPathCorners[i];
            }
        }

        void SnapToGround(Vector3 worldPosition)
        {
            Vector3 rayOrigin = worldPosition + (Vector3.up * GroundProbeHeight);
            int hitCount = Physics.RaycastNonAlloc(
                rayOrigin,
                Vector3.down,
                groundHits,
                GroundProbeDistance,
                ~0,
                QueryTriggerInteraction.Ignore);

            float closestDistance = float.MaxValue;
            bool foundGround = false;
            Vector3 groundedPosition = worldPosition;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = groundHits[i];

                if (!uSimRTS_WorldSurface.IsWalkable(hit))
                    continue;

                if (hit.distance >= closestDistance)
                    continue;

                closestDistance = hit.distance;
                groundedPosition.y = hit.point.y;
                foundGround = true;
            }

            transform.position = foundGround ? groundedPosition : worldPosition;
        }
        
        public LayerMask avoidanceMask;
        void CheckClearPath(Vector3 dir)
        {
            if (!waitingOnBlock)
                StartCoroutine(WaitAndClearBlock());

            Ray ray = new Ray(transform.position + (Vector3.up * size /2), transform.forward);
            int hitCount = Physics.SphereCastNonAlloc(
                ray,
                size,
                obstacleHits,
                size,
                avoidanceMask,
                QueryTriggerInteraction.Ignore);

            bool foundBlocker = false;
            float closestDistance = float.MaxValue;
            RaycastHit closestHit = new RaycastHit();

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = obstacleHits[i];

                if (hit.collider.transform.IsChildOf(transform))
                    continue;

                if (uSimRTS_WorldSurface.IsWalkable(hit) ||
                    uSimRTS_WorldSurface.IsTerrainSurface(hit.collider) ||
                    uSimRTS_WorldSurface.IsRoadSupport(hit.collider) ||
                    uSimRTS_WorldSurface.IsWater(hit.collider) ||
                    hit.collider.CompareTag("blueUnit"))
                {
                    continue;
                }

                if (hit.distance >= closestDistance)
                    continue;

                foundBlocker = true;
                closestDistance = hit.distance;
                closestHit = hit;
            }

            if (!foundBlocker)
                return;

            blockingObject = closestHit.collider.gameObject;
            lastHitPos = closestHit.point;

        }

        bool waitingOnBlock = false;
        IEnumerator WaitAndClearBlock()
        {
            waitingOnBlock = true;

                yield return new WaitForSeconds(2f);
                blockingObject = null;

            waitingOnBlock = false;
        }

        public bool colliding;

        void OnTriggerStay (Collider other)
        {
            if (other.tag == "unpassable")
            {
                if (transform.InverseTransformPoint(other.transform.position).z > 0f)
                    colliding = true;
                if (transform.InverseTransformPoint(other.transform.position).z < 0f)
                    colliding = false;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "unpassable")
                colliding = false;
        }
    }
}
