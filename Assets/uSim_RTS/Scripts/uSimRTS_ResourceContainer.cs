using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_ResourceContainer : MonoBehaviour
    {
        public int size = 5;
        public int totalValue;
        public int currentCargoSize;
        public GameObject[] uiCargoFills;


        public void UpdateCargoUi()
        {
            for (int i = 0; i < size; i++)
            {
                if (i < currentCargoSize)
                    uiCargoFills[i].SetActive(true);
                else
                    uiCargoFills[i].SetActive(false);
            }
        }
    }
}
