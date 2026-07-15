using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace uSimRTS
{
    public class uSimRTS_Manager : MonoBehaviour
    {
        [Tooltip("Self static reference for easy acces.")]
        public static uSimRTS_Manager instance;
        
        public  enum Sides { Blue, Red, Green, Yellow};

        [Tooltip("To what side the player belongs?")]
        public Sides playerSide;
        [System.Serializable]
        public class CommanderCoallision
        {
            [Tooltip("Side this commander belongs to.")]
            public Sides side;
            [Tooltip("Reference to the commander object.")]
            public uSimRTS_Commander commander;
        }

        [Tooltip("Commanders in mission.")]
        public CommanderCoallision[] commanders;
        [Tooltip("Player Credits text object.")]
        public Text playerCreditsText;
        [Tooltip("Commander object of the player.")]
        public uSimRTS_Commander player;

        
        public uSimRTS_Commander GetCommanderBySide (Sides side)
        {
            uSimRTS_Commander commander = null;

            foreach (CommanderCoallision com in commanders)
            {
                if (com.side == side)
                    return com.commander;
            }

            return commander;
        }

        // Start is called before the first frame update
        void Awake()
        {
           
            instance = this;
        }

        private void Start()
        {
            UpdateRefs();
           

        }

        uSimRTS_EconomyManager playerEconomy;
        // Update is called once per frame
        void LateUpdate()
        {
            UpdateRefs();
            
        }

        void UpdateRefs()
        {

            
            //Sets the player commander
            if (player == null)
                GetCommanderBySide(playerSide);

            //Sets all commander sides attributes
            foreach (CommanderCoallision com in commanders)
            {
                if (com.commander.GetComponent<uSimRTS_CommandAI>())
                com.commander.GetComponent<uSimRTS_CommandAI>().side = com.side;
                if(com.commander.GetComponent<uSimRTS_Commander>())
                com.commander.GetComponent<uSimRTS_Commander>().side = com.side;
            }


            if (playerEconomy == null)
                playerEconomy = GetCommanderBySide(playerSide).GetComponent<uSimRTS_EconomyManager>();

            if (playerEconomy)
                playerCreditsText.text = "CREDITS: $" + playerEconomy.currentCredits.ToString();
        }
    }
}
