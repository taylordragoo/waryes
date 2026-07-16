using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimRTS
{  

    public class uSimRTS_ResourceCollector : MonoBehaviour
    {
        public uSimRTS_Unit unit;
        public uSimRTS_ResourceContainer container;
        public uSimRTS_RecolectableResource recolectableResource;
        public Animator animator;
        uSimRTS_Refinery closestRefinery = null;
        private void Start()
        {
            if (!unit)
                unit = GetComponent<uSimRTS_Unit>();
            if (!container)
                container = GetComponent<uSimRTS_ResourceContainer>();
            if(!animator)
                animator = GetComponentInChildren<Animator>();
        }

        private void FixedUpdate()
        {
            if(rotate)
                transform.rotation = Quaternion.Lerp(transform.rotation, tRot, Time.deltaTime * 7f);
        }

        void GetClosestResource()
        {
            float maxDist = 100f;
            float lastDist = maxDist;
            uSimRTS_RecolectableResource[] recolectables = GameObject.FindObjectsOfType<uSimRTS_RecolectableResource>();
            foreach (uSimRTS_RecolectableResource recolectable in recolectables)
            {
                float dist = Vector3.Distance(transform.position, recolectable.transform.position);
                if (dist < lastDist)
                {
                    recolectableResource = recolectable;
                    lastDist = dist;                    
                }
            }
        }

        uSimRTS_Refinery GetClosestRefinery()
        {
            float maxDist = 100f;
            float lastDist = maxDist;
            uSimRTS_Refinery[] refineries = GameObject.FindObjectsOfType<uSimRTS_Refinery>();           
            foreach (uSimRTS_Refinery refinery in refineries)
            {
                if (refinery.owner.side == unit.radar.side)
                {
                    float dist = Vector3.Distance(transform.position, refinery.transform.position);
                    if (dist < lastDist)
                    {
                        closestRefinery = refinery;
                        lastDist = dist;
                    }
                }
            }

            return closestRefinery;
        }

        private void LateUpdate()
        {
            if (container.currentCargoSize < container.size)
                GetClosestResource();
            else
            {
                GetClosestRefinery();

                if(closestRefinery)
                {
                    GoToRefinery(closestRefinery);
                }

            }

            

            if (recolectableResource)
            {
                unit.SetDestination(recolectableResource.transform.position);
                float dist = Vector3.Distance(transform.position, recolectableResource.transform.position);
                if (dist < 0.4f)
                    if(!collecting)
                        StartCoroutine(DoCollectResource());

            }
        }

        public void GoToRefinery (uSimRTS_Refinery refinery)
        {
            unit.SetDestination(refinery.unloadPoint.position);
            float dist = Vector3.Distance(transform.position, refinery.unloadPoint.position);
            if (dist < 0.2f)
            {
                unit.SetDestination(transform.position);
                if (!dumping)
                    StartCoroutine(DoDumpResources());
            }
        }

        bool collecting;

        IEnumerator DoCollectResource ()
        {
            collecting = true;
            if (animator)
                animator.SetBool("load", true);

            yield return new WaitForSeconds(2);

            if (animator)
                animator.SetBool("load", false);

            CollectCurrentResource();

            collecting = false;
        }

        void CollectCurrentResource ()
        {
            if (recolectableResource == null)
                return;

            if (container.currentCargoSize < container.size)
            {
                container.currentCargoSize++;
                container.totalValue += recolectableResource.value;
                Destroy(recolectableResource.gameObject);
                recolectableResource = null;
                container.UpdateCargoUi();
            }
        }


        bool dumping;
        Quaternion tRot;
        bool rotate;
        IEnumerator DoDumpResources()
        {
            dumping = true;

            tRot = Quaternion.LookRotation(closestRefinery.unloadPoint.forward, Vector3.up);

            rotate = true;
            if (animator)
                animator.SetBool("unload", true);
            yield return new WaitForSeconds(1);

            if (animator)
                animator.SetBool("unload", false);
            yield return new WaitForSeconds(2);
            rotate = false;

          
            DumpResources();

           

            dumping = false;
        }

        void DumpResources()
        {
            closestRefinery.CollectDump(container.totalValue);
            
                container.currentCargoSize = 0;
                container.totalValue = 0;               
                container.UpdateCargoUi();

            closestRefinery = null;

        }

    }
}
