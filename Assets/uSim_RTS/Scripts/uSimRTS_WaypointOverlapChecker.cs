using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_WaypointOverlapChecker : MonoBehaviour
    {
        const int MaxOffsetAttempts = 24;
        const int MaxResolutionPasses = 3;

        public uSimRTS_Unit ownerUnit;
        public LayerMask castMask;
        public void CheckWaypointOverlaping ()
        {

            StartCoroutine(WaitAndCheckOverlap());
            
        }

        IEnumerator WaitAndCheckOverlap()
        {
            for (int pass = 0; pass < MaxResolutionPasses; pass++)
            {
                yield return new WaitForFixedUpdate();

                if (!HasBlockingOverlap(transform.position))
                    yield break;

                OffsetWaypoint();
            }
        }

        Collider[] CheckOverlapAtPosition (Vector3 pos)
        {
            float s = ownerUnit.size;
            Vector3 size = new Vector3(s, s, s);
            Collider[] colliders = Physics.OverlapBox(pos, size, Quaternion.identity, castMask, QueryTriggerInteraction.Collide);

            return colliders;
        }

        bool HasBlockingOverlap(Vector3 position)
        {
            Collider[] colliders = CheckOverlapAtPosition(position);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject == gameObject)
                    continue;

                if (ownerUnit != null && collider.transform.IsChildOf(ownerUnit.transform))
                    continue;

                return true;
            }

            return false;
        }

        void OffsetWaypoint ()
        {
            Vector3 originalPosition = transform.position;

            for (int attempt = 0; attempt < MaxOffsetAttempts; attempt++)
            {
                Vector3 candidate = originalPosition + GetRandomPlace();

                if (HasBlockingOverlap(candidate))
                    continue;

                transform.position = candidate;
                return;
            }

        }

        Vector3 GetRandomPlace ()
        {
           return new Vector3 (ownerUnit.size* Random.Range(-1.5f, 1.5f), 0f, ownerUnit.size* Random.Range(-1.5f, 1.5f));
        }
    }
}
