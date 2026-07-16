using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_UnitAI : MonoBehaviour
    {
        public uSimRTS_CommandAI commander;
        uSimRTS_UnitRadar radar;
        uSimRTS_Unit unit;
        // Start is called before the first frame update
        void Start()
        {
            radar = GetComponent<uSimRTS_UnitRadar>();
            unit = GetComponent<uSimRTS_Unit>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            try
            {
                if (radar.contacts.Count > 0)
                {
                    Vector3 dest = transform.position;
                    if (Vector3.Distance (transform.position, radar.contacts[0].transform.position) > radar.range)
                          dest = radar.contacts[0].transform.position + ((transform.position - radar.contacts[0].transform.position).normalized * radar.range * 0.85f);
                    unit.SetDestination(dest);

                    unit.waypointOverlapChecker.CheckWaypointOverlaping();
                }
            }
            catch
            {

            }
           
        }
    }
}
