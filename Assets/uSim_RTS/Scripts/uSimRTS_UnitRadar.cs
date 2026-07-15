using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_UnitRadar : MonoBehaviour
    {
        [Tooltip("Which side is this radar belongs to")]
        public uSimRTS_Manager.Sides side;
        [Tooltip("Radar range in meters")]
        public float range;
        [Tooltip("visual range in meters")]
        public float visRange = 2f;
        [Tooltip("Other radars belonging to enemie side. Internal use.")]
        public List<uSimRTS_UnitRadar> enemies;
        [Tooltip("Close objects. internal use.")]
        public List<GameObject> contacts;
        [Tooltip("Indicate the meshes that need to be turned off when unit is hided.")]
        public GameObject[] rendererObjects;
        // Start is called before the first frame update
        IEnumerator Start()
        {
            HideUnit();
            yield return new WaitForSeconds(1);
            if (side == uSimRTS_Manager.instance.playerSide && GetComponent<uSimRTS_UnitAI>() != null)
                GetComponent<uSimRTS_UnitAI>().enabled = false;

            if (side == uSimRTS_Manager.instance.playerSide)
                ShowUnit();

            InvokeRepeating("GetEnemiesInRange", 1f, Random.Range(1f, 3f));

        }

        // Update is called once per frame
        void LateUpdate()
        {
            // GetEnemiesInRange();

            if (GetComponent<uSimRTS_CombatController>())
                if (GetComponent<uSimRTS_CombatController>().target == null)
                    if (enemies.Count > 0)
                    {
                        if(contacts[0] != null)
                            GetComponent<uSimRTS_CombatController>().SetTarget(contacts[0].transform);
                    }

        }

        void GetEnemiesInRange ()
        {
            enemies.Clear();
            contacts.Clear();
            uSimRTS_UnitRadar[] units = GameObject.FindObjectsOfType<uSimRTS_UnitRadar>() as uSimRTS_UnitRadar[];

            foreach(uSimRTS_UnitRadar unit in units)
            {
                if (unit.side != side)
                    if (Vector3.Distance(transform.position, unit.transform.position) <= visRange)
                    {
                        contacts.Add(unit.gameObject);
                        unit.ShowUnit();
                        if (Vector3.Distance(transform.position, unit.transform.position) <= range)
                            enemies.Add(unit);
                    }
            }

        }

        public void HideUnit ()
        {
            foreach (GameObject obj in rendererObjects)
                obj.SetActive(false);
        }

        public void ShowUnit()
        {
            foreach (GameObject obj in rendererObjects)
                obj.SetActive(true);
        }
    }
}
