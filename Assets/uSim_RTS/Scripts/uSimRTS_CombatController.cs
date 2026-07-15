using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace uSimRTS
{
    public class uSimRTS_CombatController : MonoBehaviour
    {
        [System.Serializable]
        public class UnitWeapon
        {
            [Tooltip("Id")]
            public string name;
            [Tooltip("seconds to reload")]
            public float reloadTime;
            [Tooltip("The spawn point of both ammo and FX")]
            public Transform firePoint;
            [Tooltip("the ammo prefab to be spawned.")]
            public GameObject ammoPrefab;
            [Tooltip("the fire FX prefab to be spawned.")]
            public GameObject fireFx;
            [Tooltip("Is ready to fire? internal use")]
            public bool canFire;
            [Tooltip("Does it play an animation when firing? used for example for soldiers")]
            public bool useAnimation;
        }
        [Tooltip("The turret object to rotate.")]
        public Transform turret;
        [Tooltip("The weapons available in this unit. Internal use")]
        public UnitWeapon[] weapons;
        [Tooltip("The target object. Internal use")]
        public Transform target;
        public uSimRTS_UnitRadar radar;
        public bool firing;
        // Start is called before the first frame update
        void Start()
        {
            radar = GetComponent<uSimRTS_UnitRadar>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //if no target look ahead.
            if (!target)
            {
                turret.rotation = Quaternion.Lerp(turret.rotation, Quaternion.LookRotation((transform.position + transform.forward) - transform.position, transform.up), Time.deltaTime);
                return;
            }

            // look to the target.
            turret.rotation = Quaternion.Lerp(turret.rotation, Quaternion.LookRotation(target.position - transform.position, transform.up), Time.deltaTime);

   
            //if we are aiming the target, fire.
            var t = target.position;
            t.y = turret.transform.position.y;
            if (Vector3.Angle(t - turret.transform.position, turret.forward) < 3f)
            {
                firing = true;
                FireWeapon(0);
                if(weapons.Length > 1)
                FireWeapon(1);
            }
        }

        public void FireWeapon (int index)
        {            
            UnitWeapon weaponToFire = weapons[index];

            if (!weaponToFire.canFire)
                return;

            if (Vector3.Distance(target.position, transform.position) > radar.range)
                return;

            if (weaponToFire.useAnimation)
                if (GetComponent <uSimRTS_CharacterAnimations>())
                    GetComponent <uSimRTS_CharacterAnimations>().FireAnim();

            GameObject ammo = Instantiate(weaponToFire.ammoPrefab, weaponToFire.firePoint.position, weaponToFire.firePoint.rotation);

            if (weaponToFire.fireFx != null)
                Instantiate(weaponToFire.fireFx, weaponToFire.firePoint.position, weaponToFire.firePoint.rotation);

            if (ammo.GetComponent<uSimRTS_BalisticAmmo>() != null)
                ammo.GetComponent<uSimRTS_BalisticAmmo>().target = target;

                StartCoroutine(WaitAndReload(weaponToFire));
        }

        IEnumerator WaitAndReload (UnitWeapon weapon)
        {
            weapon.canFire = false;
            yield return new WaitForSeconds(weapon.reloadTime);
            firing = false;
            weapon.canFire = true;
        }

        public void SetTarget (Transform tgt)
        {
            target = tgt;
        }

    }
}
