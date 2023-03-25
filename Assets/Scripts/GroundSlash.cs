using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlash : MonoBehaviour
{
    public float speed = 15f;
    public float slowDownRate = 0.01f;
    public float detectingDistance = 3f;
    public float destoryDelay = 5f;

    private Rigidbody rb;
    public bool stopped;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if(GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
            StartCoroutine(SlowDown());

            Destroy(gameObject, destoryDelay);
        }
    }

    private void FixedUpdate()
    {
        if(stopped)
        {
            RaycastHit hit;
            Vector3 distance = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            if (Physics.Raycast(distance, transform.TransformDirection(-Vector3.up), out hit, detectingDistance))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
            else
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    IEnumerator SlowDown()
    {
        float t = 1f;
        while(t>0)
        {
            rb.velocity = Vector3.Lerp(Vector3.zero, rb.velocity, t);
            t -= slowDownRate;
            yield return new WaitForSeconds(0.1f);
        }
        stopped = true;
    }
}
