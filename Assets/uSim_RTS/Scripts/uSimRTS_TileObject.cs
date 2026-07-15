using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_TileObject : MonoBehaviour
    {
        public float radius;
        public LayerMask castMask;
        List<GameObject> buildingsInTile;
        // Start is called before the first frame update
        void Start()
        {
            GetBuildingsInTile();
            ToggleBuildingsInTile(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void GetBuildingsInTile ()
        {
            Collider[] buildingsInTileColliders = CheckOverlapAtPosition(transform.position);

            buildingsInTile = new List<GameObject>();

            if (buildingsInTileColliders.Length > 0)
            {
                foreach (Collider col in buildingsInTileColliders)
                {
                    buildingsInTile.Add(col.gameObject);
                }
            }
        }

        public void ToggleBuildingsInTile (bool set)
        {
          
            if (buildingsInTile.Count > 0)
            {
                foreach (GameObject go in buildingsInTile)
                {
                   go.SetActive(set);
                }
            }
        }

        Collider[] CheckOverlapAtPosition(Vector3 pos)
        {
            Collider[] colliders = Physics.OverlapSphere(pos, radius, castMask, QueryTriggerInteraction.Collide);

            return colliders;
        }
    }
}
