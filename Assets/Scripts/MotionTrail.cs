using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrail : MonoBehaviour
{
    public float activeTime = 2f;

    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public Transform positionToSpawn;
    public float meshDestroyDelay = 3f;

    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    [Header("Shader Related")]
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));

        }
    }

    IEnumerator ActivateTrail(float timeActive)
    {
        while(timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i = 0; i < skinnedMeshRenderers.Length; ++i)
            {
                GameObject gObj = new GameObject();
                
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
                gObj.transform.localScale = positionToSpawn.localScale;

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
               
                mr.shadowCastingMode = 0;
                
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                
                skinnedMeshRenderers[i].BakeMesh(mesh);
               
         
                mf.mesh = mesh;
                mr.material = mat;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));
                Destroy(gObj, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        float valuetoAnimate = mat.GetFloat(shaderVarRef);

        while(valuetoAnimate > goal)
        {
            valuetoAnimate -= rate;
            mat.SetFloat(shaderVarRef, valuetoAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
