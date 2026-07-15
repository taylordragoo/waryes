using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uSimRTS;

public class uSimRTS_CharacterAnimations : MonoBehaviour
{
    public Animator animator;
   

    Vector3 lastPos;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        if(animator == null)
         animator = GetComponent<Animator>();
       

        InvokeRepeating("UpdateSpeed", 0f, 0.25f);

    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null)
            return;

        animator.SetFloat("speed", speed * 10f);
    }


    void UpdateSpeed ()
    {
        speed = Vector3.Distance(transform.position, lastPos);
        lastPos = transform.position;
    }

    public void FireAnim()
    {
        if (animator == null)
            return;

        animator.SetBool("fire", true);

        StartCoroutine(WaitAndReleaseFire());
    }

    IEnumerator WaitAndReleaseFire()
    {
        yield return new WaitForSeconds(1);

        animator.SetBool("fire", false);
    }
}
