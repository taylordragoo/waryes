using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace uSimRTS
{
    internal enum uSimRTS_RouteDebugKind
    {
        Direct,
        Terrain,
        Road
    }

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
        [Tooltip("Additional cost for road shoulders and intersections relative to the RoadCenter ribbon. Higher values hold vehicles closer to the middle of gentle curves.")]
        [Min(1f)] public float roadShoulderCostMultiplier = 1.2f;
        [Tooltip("Relative off-road travel time. Higher values make vehicles accept longer road routes.")]
        [Min(1f)] public float terrainPathCost = 4f;
        [Tooltip("Maximum distance used to project the vehicle and destination onto the NavMesh.")]
        [Min(0.1f)] public float roadPathSampleDistance = 12f;
        [Tooltip("Maximum distance from either endpoint at which a road entrance or exit may be considered.")]
        [Min(1f)] public float roadAccessSearchDistance = 200f;
        [Tooltip("Ignore road routes whose on-road portion is shorter than this distance.")]
        [Min(0f)] public float minimumRoadTravelDistance = 20f;
        [Tooltip("Allows a road route to cost slightly more than the estimated direct route so road use remains predictable.")]
        [Min(1f)] public float maximumRoadRouteCostMultiplier = 1.35f;
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
        const string RoadCenterAreaName = "RoadCenter";
        const string TerrainAreaName = "Walkable";
        const string VehicleObstacleLayerName = "VehicleObstacle";
        const string RoadDecorationLayerName = "RoadDecoration";
        const string BuildingsLayerName = "buildings";
        const string UnpassableLayerName = "unpassable";
        const string UnitsLayerName = "units";
        const float DestinationChangeThreshold = 0.05f;
        const int CurrentRoadNavigationSettingsVersion = 2;
        const float DefaultRoadPathCost = 1f;
        const float DefaultRoadShoulderCostMultiplier = 1.2f;
        const float DefaultTerrainPathCost = 4f;
        const float DefaultRoadPathSampleDistance = 12f;
        const float DefaultRoadAccessSearchDistance = 200f;
        const float DefaultMinimumRoadTravelDistance = 20f;
        const float DefaultMaximumRoadRouteCostMultiplier = 1.35f;
        const float DefaultRoadPathCornerDistance = 0.75f;
        const float DefaultRoadPathRecalculationDelay = 0.25f;
        const float DefaultMinimumBlockingObstacleWidthRatio = 0.75f;

        NavMeshPath roadPath;
        NavMeshPath terrainPath;
        Vector3[] roadPathCorners = System.Array.Empty<Vector3>();
        readonly List<Vector3> calculatedRoutePoints = new List<Vector3>(128);
        int roadPathCornerIndex;
        Vector3 evaluatedRoadPathDestination;
        bool hasEvaluatedRoadPathDestination;
        bool currentRouteUsesRoads;
        float nextRoadPathCalculationTime;

        [SerializeField, HideInInspector]
        int roadNavigationSettingsVersion = CurrentRoadNavigationSettingsVersion;

        const float GroundProbeHeight = 100f;
        const float GroundProbeDistance = 250f;
        readonly RaycastHit[] groundHits = new RaycastHit[32];
        readonly RaycastHit[] obstacleHits = new RaycastHit[32];
        int vehicleObstacleLayer = -1;
        int roadDecorationLayer = -1;
        int buildingsLayer = -1;
        int unpassableLayer = -1;
        int unitsLayer = -1;

        void Awake()
        {
            EnsureRoadNavigationSettings();
            uSimRTS_ProjectScale.ConfigureUnit(this);
            uSimRTS_ProjectScale.ConfigureRanges(this);

            if (GetComponent<uSimRTS_UnitPathVisualizer>() == null)
                gameObject.AddComponent<uSimRTS_UnitPathVisualizer>();
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
                terrainPath = new NavMeshPath();
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
            int roadCenterArea = NavMesh.GetAreaFromName(RoadCenterAreaName);
            int terrainArea = NavMesh.GetAreaFromName(TerrainAreaName);

            if (roadArea < 0 || terrainArea < 0)
                return;

            EnsureRoadNavigationSettings();

            NavMeshQueryFilter terrainFilter = new NavMeshQueryFilter
            {
                agentTypeID = roadPathAgentTypeId,
                areaMask = 1 << terrainArea
            };

            if (!NavMesh.SamplePosition(
                    transform.position,
                    out NavMeshHit startTerrainHit,
                    roadPathSampleDistance,
                    terrainFilter) ||
                !NavMesh.SamplePosition(
                    destination,
                    out NavMeshHit destinationTerrainHit,
                    roadPathSampleDistance,
                    terrainFilter))
            {
                return;
            }

            if (terrainPath == null)
                terrainPath = new NavMeshPath();

            if (roadPath == null)
                roadPath = new NavMeshPath();

            bool hasTerrainPath = TryCalculateCompletePath(
                startTerrainHit.position,
                destinationTerrainHit.position,
                terrainFilter,
                terrainPath);

            float directTerrainDistance = hasTerrainPath
                ? CalculatePathLength(terrainPath.corners)
                : PlanarDistance(transform.position, destination);

            if (TryBuildRoadRoute(
                    destination,
                    roadArea,
                    roadCenterArea,
                    directTerrainDistance))
            {
                return;
            }

            if (!hasTerrainPath)
                return;

            calculatedRoutePoints.Clear();
            AppendPathCorners(calculatedRoutePoints, terrainPath.corners);
            AddDebugRoutePoint(calculatedRoutePoints, destination);
            StoreCalculatedRoute(false);
        }

        bool TryBuildRoadRoute(
            Vector3 destination,
            int roadArea,
            int roadCenterArea,
            float directTerrainDistance)
        {
            int roadAreaMask = 1 << roadArea;
            if (roadCenterArea >= 0)
                roadAreaMask |= 1 << roadCenterArea;

            NavMeshQueryFilter roadFilter = new NavMeshQueryFilter
            {
                agentTypeID = roadPathAgentTypeId,
                areaMask = roadAreaMask
            };

            roadFilter.SetAreaCost(
                roadArea,
                roadPathCost * roadShoulderCostMultiplier);

            if (roadCenterArea >= 0)
                roadFilter.SetAreaCost(roadCenterArea, roadPathCost);

            if (!NavMesh.SamplePosition(
                    transform.position,
                    out NavMeshHit roadEntry,
                    roadAccessSearchDistance,
                    roadFilter) ||
                !NavMesh.SamplePosition(
                    destination,
                    out NavMeshHit roadExit,
                    roadAccessSearchDistance,
                    roadFilter))
            {
                return false;
            }

            if (!TryCalculateCompletePath(
                    roadEntry.position,
                    roadExit.position,
                    roadFilter,
                    roadPath))
            {
                return false;
            }

            float roadDistance = CalculatePathLength(roadPath.corners);
            if (roadDistance < minimumRoadTravelDistance)
                return false;

            float entryDistance = PlanarDistance(transform.position, roadEntry.position);
            float exitDistance = PlanarDistance(roadExit.position, destination);
            float roadRouteCost =
                ((entryDistance + exitDistance) * terrainPathCost) +
                (roadDistance * roadPathCost);
            float directRouteCost = Mathf.Max(
                directTerrainDistance,
                PlanarDistance(transform.position, destination)) * terrainPathCost;

            if (roadRouteCost > directRouteCost * maximumRoadRouteCostMultiplier)
                return false;

            calculatedRoutePoints.Clear();
            AddDebugRoutePoint(calculatedRoutePoints, roadEntry.position);
            AppendPathCorners(calculatedRoutePoints, roadPath.corners);
            AddDebugRoutePoint(calculatedRoutePoints, destination);
            StoreCalculatedRoute(true);
            return true;
        }

        static bool TryCalculateCompletePath(
            Vector3 start,
            Vector3 destination,
            NavMeshQueryFilter filter,
            NavMeshPath path)
        {
            path.ClearCorners();
            return NavMesh.CalculatePath(start, destination, filter, path) &&
                path.status == NavMeshPathStatus.PathComplete &&
                path.corners != null &&
                path.corners.Length > 0;
        }

        static float CalculatePathLength(Vector3[] corners)
        {
            float length = 0f;

            for (int i = 1; i < corners.Length; i++)
                length += Vector3.Distance(corners[i - 1], corners[i]);

            return length;
        }

        static float PlanarDistance(Vector3 start, Vector3 destination)
        {
            Vector3 delta = destination - start;
            delta.y = 0f;
            return delta.magnitude;
        }

        static void AppendPathCorners(List<Vector3> points, Vector3[] corners)
        {
            if (corners == null)
                return;

            for (int i = 0; i < corners.Length; i++)
                AddDebugRoutePoint(points, corners[i]);
        }

        void StoreCalculatedRoute(bool usesRoads)
        {
            roadPathCorners = calculatedRoutePoints.ToArray();
            roadPathCornerIndex = 0;
            currentRouteUsesRoads = usesRoads;
        }

        void ClearRoadPath()
        {
            roadPathCorners = System.Array.Empty<Vector3>();
            roadPathCornerIndex = 0;
            currentRouteUsesRoads = false;
        }

        void OnValidate()
        {
            EnsureRoadNavigationSettings();
        }

        void EnsureRoadNavigationSettings()
        {
            if (roadNavigationSettingsVersion < 1)
            {
                preferRoads = !useNavMeshAgent;
                roadPathCost = Mathf.Max(DefaultRoadPathCost, roadPathCost);
                terrainPathCost = Mathf.Max(DefaultTerrainPathCost, terrainPathCost);
                roadShoulderCostMultiplier = Mathf.Max(
                    DefaultRoadShoulderCostMultiplier,
                    roadShoulderCostMultiplier);
                roadPathSampleDistance = roadPathSampleDistance > 0f
                    ? roadPathSampleDistance
                    : DefaultRoadPathSampleDistance;
                roadAccessSearchDistance = roadAccessSearchDistance > 0f
                    ? roadAccessSearchDistance
                    : DefaultRoadAccessSearchDistance;
                minimumRoadTravelDistance = DefaultMinimumRoadTravelDistance;
                maximumRoadRouteCostMultiplier = DefaultMaximumRoadRouteCostMultiplier;
                roadPathCornerDistance = roadPathCornerDistance > 0f
                    ? roadPathCornerDistance
                    : DefaultRoadPathCornerDistance;
                roadPathRecalculationDelay = roadPathRecalculationDelay > 0f
                    ? roadPathRecalculationDelay
                    : DefaultRoadPathRecalculationDelay;
                roadNavigationSettingsVersion = 1;
            }

            if (roadNavigationSettingsVersion < 2)
            {
                ignoreSmallObstacles = true;
                minimumBlockingObstacleWidthRatio = DefaultMinimumBlockingObstacleWidthRatio;
                roadNavigationSettingsVersion = 2;
            }

            roadPathCost = Mathf.Max(1f, roadPathCost);
            roadShoulderCostMultiplier = Mathf.Max(1f, roadShoulderCostMultiplier);
            terrainPathCost = Mathf.Max(1f, terrainPathCost);
            roadPathSampleDistance = Mathf.Max(0.1f, roadPathSampleDistance);
            roadAccessSearchDistance = Mathf.Max(1f, roadAccessSearchDistance);
            minimumRoadTravelDistance = Mathf.Max(0f, minimumRoadTravelDistance);
            maximumRoadRouteCostMultiplier = Mathf.Max(1f, maximumRoadRouteCostMultiplier);
            roadPathCornerDistance = Mathf.Max(0.1f, roadPathCornerDistance);
            roadPathRecalculationDelay = Mathf.Max(0f, roadPathRecalculationDelay);
            minimumBlockingObstacleWidthRatio = Mathf.Clamp(
                minimumBlockingObstacleWidthRatio,
                0.1f,
                1.5f);
            ConfigureObstacleLayerMask();
        }

        internal bool TryBuildDebugRoute(
            List<Vector3> points,
            out uSimRTS_RouteDebugKind routeKind)
        {
            points.Clear();
            routeKind = uSimRTS_RouteDebugKind.Direct;

            if (useNavMeshAgent)
            {
                if (navMeshAgent == null ||
                    !navMeshAgent.isOnNavMesh ||
                    !navMeshAgent.hasPath)
                {
                    return false;
                }

                Vector3 agentDestination = navMeshAgent.destination;
                if (PlanarDirectionTo(agentDestination).sqrMagnitude <= size * size)
                    return false;

                AddDebugRoutePoint(points, transform.position);

                Vector3[] agentCorners = navMeshAgent.path.corners;
                if (agentCorners != null && agentCorners.Length > 1)
                    routeKind = uSimRTS_RouteDebugKind.Terrain;

                if (agentCorners != null)
                {
                    for (int i = 0; i < agentCorners.Length; i++)
                        AddDebugRoutePoint(points, agentCorners[i]);
                }

                AddDebugRoutePoint(points, agentDestination);
                return points.Count > 1;
            }

            if (waypoint == null)
                return false;

            Vector3 destination = waypoint.position;
            if (PlanarDirectionTo(destination).sqrMagnitude <= size * size)
                return false;

            AddDebugRoutePoint(points, transform.position);

            if (roadPathCorners != null && roadPathCornerIndex < roadPathCorners.Length)
            {
                routeKind = currentRouteUsesRoads
                    ? uSimRTS_RouteDebugKind.Road
                    : uSimRTS_RouteDebugKind.Terrain;

                for (int i = roadPathCornerIndex; i < roadPathCorners.Length; i++)
                    AddDebugRoutePoint(points, roadPathCorners[i]);
            }

            AddDebugRoutePoint(points, destination);
            return points.Count > 1;
        }

        static void AddDebugRoutePoint(List<Vector3> points, Vector3 point)
        {
            const float duplicatePointDistanceSquared = 0.0001f;

            if (points.Count > 0 &&
                (points[points.Count - 1] - point).sqrMagnitude <= duplicatePointDistanceSquared)
            {
                return;
            }

            points.Add(point);
        }

        void OnDrawGizmosSelected()
        {
            if (roadPathCorners == null || roadPathCorners.Length == 0)
                return;

            Gizmos.color = currentRouteUsesRoads ? Color.cyan : Color.yellow;
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
        
        [Header("Local Obstacle Avoidance")]
        public LayerMask avoidanceMask;
        [Tooltip("Ignore unclassified props whose collider footprint is small compared with this unit.")]
        public bool ignoreSmallObstacles = true;
        [Tooltip("Minimum obstacle width and depth, as a fraction of the unit diameter, required for an unclassified prop to block movement.")]
        [Range(0.1f, 1.5f)]
        public float minimumBlockingObstacleWidthRatio = DefaultMinimumBlockingObstacleWidthRatio;

        void ConfigureObstacleLayerMask()
        {
            vehicleObstacleLayer = LayerMask.NameToLayer(VehicleObstacleLayerName);
            roadDecorationLayer = LayerMask.NameToLayer(RoadDecorationLayerName);
            buildingsLayer = LayerMask.NameToLayer(BuildingsLayerName);
            unpassableLayer = LayerMask.NameToLayer(UnpassableLayerName);
            unitsLayer = LayerMask.NameToLayer(UnitsLayerName);

            if (vehicleObstacleLayer >= 0)
                avoidanceMask |= 1 << vehicleObstacleLayer;

            if (roadDecorationLayer >= 0)
                avoidanceMask &= ~(1 << roadDecorationLayer);
        }

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

                if (ShouldIgnoreLocalObstacle(hit.collider))
                    continue;

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

        bool ShouldIgnoreLocalObstacle(Collider obstacle)
        {
            if (obstacle == null)
                return true;

            int obstacleLayer = obstacle.gameObject.layer;
            if (obstacleLayer == roadDecorationLayer)
                return true;

            if (obstacleLayer == vehicleObstacleLayer ||
                obstacleLayer == buildingsLayer ||
                obstacleLayer == unpassableLayer ||
                obstacleLayer == unitsLayer ||
                obstacle.CompareTag("unpassable") ||
                obstacle.GetComponentInParent<uSimRTS_BaseBuilding>() != null)
            {
                return false;
            }

            if (!ignoreSmallObstacles)
                return false;

            Bounds bounds = obstacle.bounds;
            float unitDiameter = Mathf.Max(size * 2f, 0.1f);
            float minimumBlockingWidth = unitDiameter * minimumBlockingObstacleWidthRatio;
            bool hasBlockingFootprint = bounds.size.x >= minimumBlockingWidth &&
                                        bounds.size.z >= minimumBlockingWidth;
            bool isWallLike = Mathf.Max(bounds.size.x, bounds.size.z) >= unitDiameter &&
                              bounds.size.y >= size;

            return !hasBlockingFootprint && !isWallLike;
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
