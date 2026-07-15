using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace uSimRTS
{
    public class uSimRTS_BuildButtonUi : MonoBehaviour
    {
        public Button button;
        [Tooltip("Text component of the button")]
        public Text buttonText;
        [Tooltip("Text component of the cost")]
        public Text costText;
        [Tooltip("Index of unit to spawn by spwner")]
        public int unitIndex;
        [Tooltip("Player commander")]
        public uSimRTS_Commander player;
        [Tooltip("units spawner")]
        public uSimRTS_UnitSpawner uSimRTS_UnitSpawner;
        uSimRTS_BuildingOptions uSimRTS_BuildingOptions;

        private void Start()
        {
            button = GetComponent<Button>();
        }

        public void SetButtonData(string name, int cost, uSimRTS_UnitSpawner spawner, int index)
        {
            buttonText.text = name;
            costText.text = cost.ToString();
            uSimRTS_UnitSpawner = spawner;
            unitIndex = index;
            GetComponent<Button>().onClick.AddListener(this.SpawnUnit);
        }

        public void SetBuildButtonData(string name, int cost, uSimRTS_BuildingOptions option, int index)
        {
            buttonText.text = name;
            costText.text = cost.ToString();
            uSimRTS_BuildingOptions = option;
            unitIndex = index;
            GetComponent<Button>().onClick.AddListener(this.SpawnBuilding);
        }

        public void SpawnUnit()
        {
            uSimRTS_UnitSpawner.SpawnUnitByIndex(unitIndex);
            float buildTime = uSimRTS_UnitSpawner.availableUnits[unitIndex].unitPrefab.GetComponent<uSimRTS_Unit>().buildTime;
            StartCoroutine(WaitAndRenable(buildTime));
        }

        IEnumerator WaitAndRenable(float time)
        {
            button.interactable = false;
            yield return new WaitForSeconds(time);
            button.interactable = true;
        }

        public void SpawnBuilding()
        {
            uSimRTS_BuildingOptions.SpawnByIndex(unitIndex);
            button.interactable = false;
          
        }
    }
}
