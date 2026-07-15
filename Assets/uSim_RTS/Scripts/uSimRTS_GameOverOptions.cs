using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_GameOverOptions : MonoBehaviour
    {

        public enum Condition {NoPlayerUnits, NoEnemyUnits, TargetDestroyed};
        public enum Type {Victory, Defeat};

        [System.Serializable]
        public class EndGameCondition
        {
            
            public Condition condition;
            public Type type;
            [Tooltip("1 = main menu, Or at least that's how game logic works in this case where index 0 is the loader scene and it should only be opened at start of the application")]
            public int levelToLoad; //1 = main menu, Or at least that's how game logic works in this case where index 0 is the loader scene and it should only be opened at start of the application.
        }

        public EndGameCondition[] endGameConditions;
        [Tooltip("Set a custom victory screen here")]
        public GameObject victoryScreen;
        [Tooltip("Set a custom defeat screen here")]
        public GameObject defeatScreen;
        public GameObject sideBar;
        public bool check;
        bool ended;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            if(check)
            CheckConditions();
        }

        void CheckConditions()
        {
            if (ended)
                return;

            foreach (EndGameCondition condition in endGameConditions)
            {
                switch (condition.condition)
                {

                    case Condition.NoPlayerUnits:

                        if (!PlayerHasUnits())
                            if(!PlayerHasBuildings())
                                EndGame(condition.type, condition.levelToLoad);

                        break;

                    case Condition.NoEnemyUnits:

                        if (!EnemyHasUnits())
                            if (!EnemyHasBuildings())
                                EndGame(condition.type, condition.levelToLoad);

                        break;

                }
            }
        }

        bool PlayerHasBuildings()
        {
            uSimRTS_BaseBuilding[] buildings = GameObject.FindObjectsOfType<uSimRTS_BaseBuilding>();

            List<uSimRTS_BaseBuilding> playerBuildings = new List<uSimRTS_BaseBuilding>();

            foreach (uSimRTS_BaseBuilding building in buildings)
            {
                if (building.side == uSimRTS_Manager.instance.playerSide)
                    playerBuildings.Add(building);
            }

            if (playerBuildings.Count > 0)
                return true;

            return false;
        }

        bool PlayerHasUnits()
        {
            uSimRTS_Unit[] units = GameObject.FindObjectsOfType<uSimRTS_Unit>();

            List<uSimRTS_Unit> playerUnits = new List<uSimRTS_Unit>();

            foreach (uSimRTS_Unit unit in units)
            {
                if (unit.GetComponent<uSimRTS_UnitRadar>().side == uSimRTS_Manager.instance.playerSide)
                    playerUnits.Add(unit);
            }

            if (playerUnits.Count > 0)
                return true;

            return false;
        }

        bool EnemyHasBuildings()
        {
            uSimRTS_BaseBuilding[] buildings = GameObject.FindObjectsOfType<uSimRTS_BaseBuilding>();

            List<uSimRTS_BaseBuilding> playerBuildings = new List<uSimRTS_BaseBuilding>();

            foreach (uSimRTS_BaseBuilding building in buildings)
            {
                if (building.side != uSimRTS_Manager.instance.playerSide)
                    playerBuildings.Add(building);
            }

            if (playerBuildings.Count > 0)
                return true;

            return false;
        }

        bool EnemyHasUnits()
        {
            uSimRTS_Unit[] units = GameObject.FindObjectsOfType<uSimRTS_Unit>();

            List<uSimRTS_Unit> playerUnits = new List<uSimRTS_Unit>();

            foreach (uSimRTS_Unit unit in units)
            {
                if (unit.GetComponent<uSimRTS_UnitRadar>().side != uSimRTS_Manager.instance.playerSide)
                    playerUnits.Add(unit);
            }

            if (playerUnits.Count > 0)
                return true;

            return false;
        }


        void EndGame(Type type, int scene)
        {
            ended = true;
           

            if(type == Type.Defeat)
            {
                defeatScreen.SetActive(true);
                uSimRTS_Main.instance.WaitAndLoadScene(scene);
            }
            if (type == Type.Victory)
            {
                victoryScreen.SetActive(true);
                uSimRTS_Main.instance.WaitAndLoadScene(scene);
            }

            uSimRTS_Main.instance.SaveGame();


            if (sideBar)
                sideBar.SetActive(false);

        }


       

    }
}
