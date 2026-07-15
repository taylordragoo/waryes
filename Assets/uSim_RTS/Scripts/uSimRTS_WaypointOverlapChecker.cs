using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_WaypointOverlapChecker : MonoBehaviour
    {
        public uSimRTS_Unit ownerUnit;
        public LayerMask castMask;
        public void CheckWaypointOverlaping ()
        {

            StartCoroutine(WaitAndCheckOverlap());
            
        }

        void LateUpdate ()
        {
            if (ownerUnit != null)          
            StartCoroutine(WaitAndCheckOverlap());
        }

        IEnumerator WaitAndCheckOverlap()
        {
            yield return new WaitForFixedUpdate();

            Collider[] colliders = CheckOverlapAtPosition(transform.position);

            if (colliders.Length > 0)
                if (colliders[0].gameObject != ownerUnit.gameObject && colliders[0].gameObject != gameObject)
                    OffsetWaypoint(colliders[0].transform.position);
        }

        Collider[] CheckOverlapAtPosition (Vector3 pos)
        {
            float s = ownerUnit.size / 2f;
            Vector3 size = new Vector3(s, s, s);
            Collider[] colliders = Physics.OverlapBox(pos, size, Quaternion.identity, castMask, QueryTriggerInteraction.Collide);

            return colliders;
        }

        void OffsetWaypoint (Vector3 from)
        {
            Vector3 dir = from - transform.position;
            Vector3 randomPos = transform.position;
            Collider[] colliders;

            do
            {
                randomPos = GetRandomPlace();
                colliders = CheckOverlapAtPosition(randomPos);
            }
            while (colliders.Length > 0);

            Vector3 pos = transform.position;
            pos += randomPos;
            

            transform.position = pos;

        }

        Vector3 GetRandomPlace ()
        {
           return new Vector3 (ownerUnit.size* Random.Range(-1.5f, 1.5f), 0f, ownerUnit.size* Random.Range(-1.5f, 1.5f));
        }
    }
}
