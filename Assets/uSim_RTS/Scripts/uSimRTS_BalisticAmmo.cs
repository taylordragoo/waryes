using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{
    public class uSimRTS_BalisticAmmo : MonoBehaviour
    {
        public Transform target;
        Vector3 tgtPos;
        public float muzzleSpeed;
        public float damagePoints = 25f;
        public float timer;
        public GameObject impactFx;
        Vector3 lastPos;
        // Start is called before the first frame update
        void Start()
        {
            tgtPos = target.position;
        }

        // Update is called once per frame
        void Update()
        {


            if (transform.InverseTransformPoint(tgtPos).z < 0.5f)
            {
                Detonate();
                if (target != null)
                {
                    if (target.transform.root.GetComponent<uSimRTS_DamageController>() != null)
                        target.transform.root.GetComponent<uSimRTS_DamageController>().AddDamage(damagePoints);
                }
            }
            transform.Translate(((tgtPos + Vector3.up * 0.02f) - transform.position).normalized * muzzleSpeed * Time.deltaTime, Space.World);

            timer += Time.deltaTime;



            if (target != null)                
            tgtPos = target.position;
          
          

         
        }


        public void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (timer > 0.01f)
            {
                if ( other.transform.tag == "damagable")
                {
                    other.transform.root.GetComponent<uSimRTS_DamageController>().AddDamage(damagePoints);                    
                }
                Detonate();
            }
        }

        void Detonate()
        {
            Instantiate(impactFx, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
