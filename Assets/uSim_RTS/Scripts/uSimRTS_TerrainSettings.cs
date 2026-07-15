using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_TerrainSettings : MonoBehaviour
    {

        public Terrain terrain;
        public bool forceBillboardTrees;
        void Start()
        {
            if (GetComponent<Terrain>() == null)
                Debug.LogError("Missing attached terrain!");
            else 
                terrain = GetComponent<Terrain>();

            if (terrain)
                ApplySettings();
        }

        void ApplySettings ()
        {
            if(forceBillboardTrees)
                terrain.treeBillboardDistance = 0f;
        }
    }
}
