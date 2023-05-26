using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

    public bool triggerActivate = false;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            if (other.CompareTag("Monster"))
            {
                other.gameObject.GetComponent<EnemyAI>().GetDamage();
                Debug.Log("att");

            }
        }
    }

}
