using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_BuildOptionsManager : MonoBehaviour
    {
        public List<uSimRTS_BaseBuilding> playerBuildings;
        public uSimRTS_BuildingOptions uSimRTS_BuildingOptions;
        public GameObject buttonPrefab;
        public GameObject sideBar;
        public Transform unitsUiPanelPivot;
        public Transform buildUiPanelPivot;
        public float buttonSeparation;
        List<GameObject> buttons;
        List<GameObject> buttonsBuild;
        List<string> units;
        // Start is called before the first frame update
        void Awake()
        {
            buttons = new List<GameObject>();
            buttonsBuild = new List<GameObject>();
            playerBuildings = new List<uSimRTS_BaseBuilding>();
        }

        // Update is called once per frame
       IEnumerator Start()
        {
            uSimRTS_Commander playerCommander = null;
            do {
                playerCommander = uSimRTS_Manager.instance.player;
                yield return new WaitForEndOfFrame();
            } while (playerCommander == null);

            uSimRTS_BuildingOptions = playerCommander.GetComponent<uSimRTS_BuildingOptions>();
            UpdateBuildUiOptions();
            InvokeRepeating("UpdateBuildUiOptions", 1f, 1f);
        }
      
        public void AddPlayerBuilding(uSimRTS_BaseBuilding building)
        {
            playerBuildings.Add(building);
            UpdateUnitsUiOptions();
        }

        public void RemovePlayerBuilding(uSimRTS_BaseBuilding building)
        {
            playerBuildings.Remove(building);
            UpdateUnitsUiOptions();
        }
        void ClearButtons()
        {
            foreach (GameObject button in buttons)
            {
                Destroy(button);
            }
            buttons = new List<GameObject>();
        }

        void ClearButtonsBuild()
        {
            foreach (GameObject button in buttonsBuild)
            {
                Destroy(button);
            }
            buttonsBuild = new List<GameObject>();
        }


        public void UpdateUnitsUiOptions ()
        {
            ClearButtons();
            units = new List<string>();
            int unitsCount = 0;
            foreach (uSimRTS_BaseBuilding building in playerBuildings)
            {

                for ( int i = 0; i < building.spawner.availableUnits.Length; i++) 
                {
                    uSimRTS_UnitSpawner.AvailableUnit unit = building.spawner.availableUnits[i];

                    string unitName = unit.name;
                    int value = unit.cost;
                    uSimRTS_UnitSpawner spawner = null;
                    if (building.GetComponent<uSimRTS_UnitSpawner>() != null)
                    {
                        spawner = building.GetComponent<uSimRTS_UnitSpawner>();
                        if (!UnitExist(unit.name))
                        {

                            uSimRTS_BuildButtonUi buttonUi = Instantiate(buttonPrefab, unitsUiPanelPivot).GetComponent<uSimRTS_BuildButtonUi>();
                            buttons.Add(buttonUi.gameObject);
                            Vector3 pos = Vector3.zero;
                            pos.y -= buttonSeparation * unitsCount;
                            buttonUi.transform.localPosition = pos;
                            buttonUi.player = uSimRTS_Manager.instance.player;
                            buttonUi.SetButtonData(unitName, value, spawner, i);
                            units.Add(unit.name);
                            unitsCount++;
                        }
                    }

                }
            }
        }




        bool UnitExist(string checkUnit)
        {
            foreach (string unit in units)
                if (checkUnit == unit)
                    return true;

            return false;
        }

        public void UpdateBuildUiOptions()
        {
            ClearButtonsBuild();
            int buildCount = 0;
           
                for (int i = 0; i < uSimRTS_BuildingOptions.buildingsBuildOptions.Length; i++)
                {
                    var building = uSimRTS_BuildingOptions.buildingsBuildOptions [i];
                    string buildingName = building.name;
                    int value = building.reqCredits;

                if (!uSimRTS_BuildingOptions.BuildingExist(building.prefab.GetComponent<uSimRTS_BaseBuilding>()))
                {


                    if (building.reqTechLvl <= uSimRTS_BuildingOptions.techLevel)
                    {


                        uSimRTS_BuildButtonUi buttonUi = Instantiate(buttonPrefab, buildUiPanelPivot).GetComponent<uSimRTS_BuildButtonUi>();
                        buttonsBuild.Add(buttonUi.gameObject);
                        Vector3 pos = Vector3.zero;
                        pos.y -= buttonSeparation * buildCount;
                        buttonUi.transform.localPosition = pos;
                        buttonUi.player = uSimRTS_Manager.instance.player;
                        buttonUi.SetBuildButtonData(buildingName, value, uSimRTS_BuildingOptions, i);
                        buildCount++;
                    }
                }

                }
            
        }

        public void EnableBuildingOptions()
        {
            sideBar.gameObject.SetActive(true);
        }
    }
}
