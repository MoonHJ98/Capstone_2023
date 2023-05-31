using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlashShooter: MonoBehaviour
{
    public Transform player;
    public GameObject projectile;
    public float fireRate = 4;

    private Vector3 direction;

    private GroundSlash groundSlashScript;
    private PlayerMovement playerMoveScript;


    public float pressing = 0f;
    public float shootProjectile = 3f;

    // Camera Shake Test
    public CameraShake cameraShake;
    public float duration = 0.2f;
    public float magnitude = 0.025f;

    // Start is called before the first frame update
    void Start()
    {
        playerMoveScript = gameObject.GetComponent<PlayerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        if (playerMoveScript.movementState == PlayerMovement.MovementState.DODGE)
            return;

        //if(Input.GetKey(KeyCode.Mouse1))
        //{
        //    pressing += Time.deltaTime;
        //}
        //
        //if (Input.GetKeyUp(KeyCode.Mouse1))
        //{
        //    if (pressing >= shootProjectile)
        //        ShootProjectile();
        //
        //    pressing = 0f;
        //    StartCoroutine(cameraShake.Shake(0.2f, 0.025f));
        //}
    }

    public void ShootProjectile()
    {
        direction = player.forward;
        
        InstantiateProjectile();
    }

    void InstantiateProjectile()
    {
        var projectileObj = Instantiate(projectile, player.position, player.rotation);
        groundSlashScript = projectileObj.GetComponent<GroundSlash>();
        projectileObj.GetComponent<Rigidbody>().velocity = direction * groundSlashScript.speed;
    }
}
