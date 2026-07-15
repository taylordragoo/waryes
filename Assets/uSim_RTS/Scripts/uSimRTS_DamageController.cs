using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_DamageController : MonoBehaviour
    {
        [Tooltip("The Hp bar to be updated acordingly.")]
        public Transform hpBar;
        [Tooltip("Max HP")]
        public float hp = 100f;
        float initialHP;
        [Tooltip("To be spawned on death. Leave empty for none")]
        public GameObject destroyFx;
        [Tooltip("To be spawned on death. Leave empty for none")]
        public GameObject craterFx;
        // Start is called before the first frame update
        void Start()
        {
            initialHP = hp;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddDamage (float dmg)
        {
            hp -= dmg;

            Vector3 barScale = hpBar.localScale;
            barScale.x = hp / initialHP;
            hpBar.localScale = barScale;

            if (hp <= 0f)
                DestroyUnit();
        }

        void DestroyUnit ()
        {
            if (GetComponent<uSimRTS_Unit>() != null)
            {
                if (uSimRTS_Manager.instance.playerSide == GetComponent<uSimRTS_UnitRadar>().side)
                    GameObject.FindObjectOfType<uSimRTS_UnitsCommand>().RemoveFromPlayerList(GetComponent<uSimRTS_Unit>());
                else if (GetComponent<uSimRTS_UnitAI>() != null)
                    GetComponent<uSimRTS_UnitAI>().commander.RemoveFromUnitsList(GetComponent<uSimRTS_Unit>());

                Destroy(GetComponent<uSimRTS_Unit>().waypoint.gameObject);
            }
            if (GetComponent<uSimRTS_BaseBuilding>() != null)
            {
                if (uSimRTS_Manager.instance.playerSide == GetComponent<uSimRTS_UnitRadar>().side)
                    GameObject.FindObjectOfType<uSimRTS_BuildOptionsManager>().RemovePlayerBuilding(GetComponent<uSimRTS_BaseBuilding>());
            }
            if(destroyFx)
            Instantiate(destroyFx, transform.position, Quaternion.identity);
            if(craterFx)
            Instantiate(craterFx, transform.position, Quaternion.identity);
           
            Destroy(gameObject);
        }
    }
}
