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
                pointer.rotation = Quaternion.LookRotation(waypoint.position - transform.position, transform.up);


                if (!blockingObject)
                    dirToWp = waypoint.position - transform.position;



                inverseTgtPos = transform.InverseTransformPoint(waypoint.position);



                if (blockingObject != null && moveInput > 0f)
                {
                    Vector3 inversePos = pointer.InverseTransformPoint(blockingObject.transform.position);
                    if (inversePos.z > 0f)
                    {
                        if (inversePos.x <= 0f)
                            dirToWp = lastHitPos + (pointer.right * size * 1.2f);
                        if (inversePos.x > 0f)
                            dirToWp = lastHitPos - (pointer.right * size * 1.2f);
                    }
                }


                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dirToWp, Vector3.up), Time.deltaTime * turnSpeed);
           


                    moveInput = 1f;
                distToTgt = Vector3.Distance(transform.position, waypoint.position);
                if (distToTgt < size || colliding)
                    moveInput = -3f;


                speedCoef = 1f;

                if (Mathf.Abs(inverseTgtPos.x) > 0.5f)
                    speedCoef = 0.2f;



                float curMaxSpeed = maxSpeed * speedCoef;
                speed += moveInput * (accel / 100f) * Time.deltaTime;
                speed = Mathf.Clamp(speed, 0f, curMaxSpeed / 100f);

                transform.Translate(transform.forward * speed, Space.World);
            }
            else {

                if (combatController)
                {
                    if (combatController.firing && combatController.target)
                        transform.rotation = Quaternion.LookRotation(combatController.target.position - transform.position, Vector3.up);
                    else
                        transform.rotation = Quaternion.LookRotation(navMeshAgent.steeringTarget - transform.position, Vector3.up);
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(navMeshAgent.steeringTarget - transform.position, Vector3.up);
                }
            }
        }
        
        public LayerMask avoidanceMask;
        void CheckClearPath(Vector3 dir)
        {
            if (!waitingOnBlock)
                StartCoroutine(WaitAndClearBlock());

            RaycastHit hit;
            Ray ray = new Ray(transform.position + (Vector3.up * size /2), transform.forward);
            if (Physics.SphereCast(ray, size, out hit, size, avoidanceMask))
            {
                if (hit.collider.tag != "world" && hit.collider.tag != "blueUnit")
                {
                    blockingObject = hit.collider.gameObject;
                    lastHitPos = hit.point;
                }
            }

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
