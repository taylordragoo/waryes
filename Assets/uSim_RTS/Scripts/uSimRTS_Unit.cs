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
                SnapToGround(transform.position);

            waypointOverlapChecker.CheckWaypointOverlaping();

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!useNavMeshAgent) 
            {
            
                if(moveInput > 0f)
                CheckClearPath(dirToWp);


              //  pointer.rotation = Quaternion.Lerp(pointer.rotation,, Time.deltaTime);
                Vector3 waypointDirection = PlanarDirectionTo(waypoint.position);
                if (waypointDirection.sqrMagnitude > 0.0001f)
                    pointer.rotation = Quaternion.LookRotation(waypointDirection, Vector3.up);


                if (!blockingObject)
                    dirToWp = PlanarDirectionTo(waypoint.position);



                Vector3 planarWaypoint = waypoint.position;
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
                distToTgt = PlanarDirectionTo(waypoint.position).magnitude;
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
