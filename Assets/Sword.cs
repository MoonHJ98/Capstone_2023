using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

    public bool triggerActivate = false;
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
        if (triggerActivate == false)
            return;

        if (other.CompareTag("Monster"))
        {
            other.gameObject.GetComponent<EnemyAI>().GetDamage();
            Debug.Log("att2");

        }
    }

}
