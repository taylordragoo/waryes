using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{

    public class uSimRTS_Refinery : MonoBehaviour
    {
        public float processTime;
        public float processCredits;
        public float storaged;
        public uSimRTS_Commander owner;
        public Transform unloadPoint;
        public GameObject initialHarvester;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (storaged > 0f)
            {
                if (!processing)
                {
                    StartCoroutine(ProcessBatch(storaged));
                }
            }
        }

        public void CollectDump(float amount)
        {
            storaged += amount;
        }

        bool processing;
        IEnumerator ProcessBatch(float amount)
        {
            processing = true;

            yield return new WaitForSeconds(processTime);

            AddCash(amount);
            storaged -= amount;

            processing = false;
        }

        void AddCash( float amount)
        {
            if(owner.GetComponent<uSimRTS_EconomyManager>() != null)
                owner.GetComponent<uSimRTS_EconomyManager>().AddCredits(Mathf.FloorToInt(amount));
        }


        public void Deploy()
        {
            if (initialHarvester)
            {
                initialHarvester.SetActive(true);
                initialHarvester.transform.parent = null;
            }
        }

    }
}
