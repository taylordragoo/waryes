using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{

    public class uSimRTS_EconomyManager : MonoBehaviour
    {
        [Tooltip("Credits used to produce units and buildings.")]
        public int currentCredits;       
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddCredits (int credits)
        {
            currentCredits += credits;
        }

        public void SubstractCredits (int credits)
        {
            currentCredits -= credits;
        }
    }

}
