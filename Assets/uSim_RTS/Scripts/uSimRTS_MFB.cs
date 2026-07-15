using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{

    public class uSimRTS_MFB : MonoBehaviour
    {
        public GameObject deployedObject;
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void Deploy()
        {
            deployedObject.SetActive(true);
            deployedObject.transform.parent = null;
            GetComponent<uSimRTS_Unit>().waypoint.parent = transform;
            GameObject.FindObjectOfType<uSimRTS_BuildOptionsManager>().EnableBuildingOptions();
            uSimRTS_Manager.instance.GetComponent<uSimRTS_UnitsCommand>().RemoveFromPlayerList(GetComponent<uSimRTS_Unit>());          
            Destroy(gameObject);
        }
    }
}
