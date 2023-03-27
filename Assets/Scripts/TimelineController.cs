using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public Camera mainCamera;
    public Camera cinematicCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            mainCamera.gameObject.SetActive(false);
            cinematicCamera.gameObject.SetActive(true);

            playableDirector.gameObject.SetActive(true);
            playableDirector.Play();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            End();
        }

        
    }
    public void End()
    {
        mainCamera.gameObject.SetActive(true);
        cinematicCamera.gameObject.SetActive(false);

        playableDirector.gameObject.SetActive(false);
        playableDirector.Stop();
        Debug.Log("end");
    }
}
