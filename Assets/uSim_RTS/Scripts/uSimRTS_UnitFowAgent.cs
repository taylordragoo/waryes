using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_UnitFowAgent : MonoBehaviour
    {
        [Tooltip("Radius of the fog of war clear trigger.")]
        public float radius;
        [Tooltip("The fog of war mask.")]
        public LayerMask castMask;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            if(radius != 0f)
                CheckFogOfWar();
        }

        void CheckFogOfWar ()
        {
            Collider[] colliders = CheckOverlapAtPosition(transform.position);

            if (colliders.Length > 0)
            {
                foreach(Collider col in colliders)
                {
                    DiscoverTile(col.gameObject);
                }
            }             
                   
        }

        Collider[] CheckOverlapAtPosition(Vector3 pos)
        {
            float s = radius;
           
            Collider[] colliders = Physics.OverlapSphere(pos, radius, castMask, QueryTriggerInteraction.Collide);

            return colliders;
        }

        void DiscoverTile (GameObject tile)
        {
            tile.gameObject.SetActive(false);
            tile.transform.parent.GetComponent<uSimRTS_TileObject>().ToggleBuildingsInTile(true);
        }
    }
}
