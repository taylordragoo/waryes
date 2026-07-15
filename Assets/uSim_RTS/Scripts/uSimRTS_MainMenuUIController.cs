using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{

    public class uSimRTS_MainMenuUIController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartGameCall()
        {
            uSimRTS_Main.instance.StartNewGame();
        }

        public void ContrinueGameCall()
        {
            uSimRTS_Main.instance.LoadGame();
        }
    }
}
