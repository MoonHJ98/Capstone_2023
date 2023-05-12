    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMat : MonoBehaviour
{
    public Material[] mat = new Material[3];

    int i = 0;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeMaterial();
        }
    }

    public void ChangeMaterial()
    {
        i = ++i;

        if (i == 3)
            i = 0;

        // Change Material
        gameObject.GetComponent<SkinnedMeshRenderer>().material = mat[i];
    }
}
