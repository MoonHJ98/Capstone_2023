using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coll : MonoBehaviour
{
    // Start is called before the first frame update

    public bool hitOnce = true;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            if (hitOnce)
            {
                other.gameObject.GetComponent<EnemyAI>().GetDamage();
                hitOnce = false;
            }

        }
    }
}
