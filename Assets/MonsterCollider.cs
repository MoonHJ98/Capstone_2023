using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCollider : MonoBehaviour
{
    public GameObject monster;

    bool active = false;


    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (monster.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Punch_1") ||
                monster.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Punch_2") ||
                monster.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SideRoll"))
            {

                other.gameObject.GetComponent<PlayerMovement>().GetDamage(0.25f);
                Debug.Log("att");
            }

        }
    }
}
