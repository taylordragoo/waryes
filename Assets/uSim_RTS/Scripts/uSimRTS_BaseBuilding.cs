using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_BaseBuilding : MonoBehaviour
    {
        [Tooltip("Name of the building")]
        public string buildingName;
        [Tooltip("Side the building belongs to")]
        public uSimRTS_Manager.Sides side;
        [Tooltip("Commander AI belongs to")]
        public uSimRTS_CommandAI commander;
        [Tooltip("Commander Player  belongs to")]
        public uSimRTS_Commander playerCommander;
        [Tooltip("units spawner. internal use.")]
        public uSimRTS_UnitSpawner spawner;

        void Awake()
        {
            uSimRTS_ProjectScale.ApplyToGameplayRoot(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (GetComponent<uSimRTS_UnitSpawner>() != null)
                spawner = GetComponent<uSimRTS_UnitSpawner>();

            if(uSimRTS_Manager.instance.GetCommanderBySide(side) != null)
            playerCommander = uSimRTS_Manager.instance.GetCommanderBySide(side);

            if (side == uSimRTS_Manager.instance.playerSide)
                GameObject.FindObjectOfType<uSimRTS_BuildOptionsManager>().AddPlayerBuilding(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
