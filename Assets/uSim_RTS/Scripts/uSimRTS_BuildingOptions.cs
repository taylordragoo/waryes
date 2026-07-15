using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace uSimRTS
{
    public class uSimRTS_BuildingOptions : MonoBehaviour
    {
        [Tooltip("This value unlocks buildings. if teach level >= level required, building is enabled.")]
        public int techLevel;

        [System.Serializable]
        public class BuildingsBuildOptions
        {
            public string name;
            public int reqTechLvl;
            public int reqCredits;
            public float buildTime;
            public int increaseLevel;
            public GameObject prefab;
            public GameObject uiButton;
        }

        public BuildingsBuildOptions[] buildingsBuildOptions;
        GameObject previewObject;
        public bool placing;
        public bool rotating;
        RaycastHit hit;

        List<uSimRTS_BaseBuilding> playerBuildings;
        // Start is called before the first frame update
        void Start()
        {
            playerBuildings = new List<uSimRTS_BaseBuilding>();
        }

        // UPDATE HOLDS THE PLACING HANDLE.
        void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
                return;

            Vector2 mousePosition = mouse.position.ReadValue();

            if (rotating)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                   
                    if (hit.collider.tag == "world")
                        previewObject.transform.rotation = Quaternion.LookRotation(hit.point - previewObject.transform.position , Vector3.up);


                }

                if (mouse.leftButton.wasPressedThisFrame)
                {
                    previewObject.gameObject.SendMessage("Deploy", SendMessageOptions.DontRequireReceiver);
                    previewObject = null;
                    rotating = false;

                }

            }
            if (placing)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                   
                    if (hit.collider.tag == "world")
                        previewObject.transform.position = hit.point;

                    
                }

                if (mouse.leftButton.wasPressedThisFrame)
                {
                    placing = false;
                    rotating = true;
                   
                }

             }

           
        }

        public bool SpawnByIndex (int index)
        {
            if (GetComponent<uSimRTS_EconomyManager>().currentCredits < buildingsBuildOptions[index].reqCredits)
                return false;

            GetComponent<uSimRTS_EconomyManager>().SubstractCredits(buildingsBuildOptions[index].reqCredits);

            previewObject = Instantiate(buildingsBuildOptions[index].prefab, transform.position, Quaternion.identity);
            playerBuildings.Add(previewObject.GetComponent<uSimRTS_BaseBuilding>());
            if (previewObject.GetComponent<uSimRTS_Refinery>() != null)
                previewObject.GetComponent<uSimRTS_Refinery>().owner = uSimRTS_Manager.instance.player;
          
            //This will set commander's tech level by the amount indicated by the building being spawned. 
            if (buildingsBuildOptions[index].increaseLevel > techLevel)
                techLevel = buildingsBuildOptions[index].increaseLevel;
            //update the UI acordingly.
            GameObject.FindObjectOfType<uSimRTS_BuildOptionsManager>().UpdateBuildUiOptions();

            placing = true;
            return true;
        }

        public bool BuildingExist (uSimRTS_BaseBuilding building)
        {
            foreach (uSimRTS_BaseBuilding b in playerBuildings)
                if (building.buildingName == b.buildingName)
                    return true;

            return false;
        }
    }
}
