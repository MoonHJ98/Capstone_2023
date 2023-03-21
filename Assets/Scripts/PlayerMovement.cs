using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Animator _animator;
    Camera _camera;
    CharacterController _controller;


    public float speed = 5f;
    public float runSpeed = 8f;
    public float finalSpeed;
    public bool run;

    public float move;

    public float smoothness = 7f;

    public bool toggleCameraRotation;


    private bool[] keyCheck = new bool[4];
    // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _camera = Camera.main;
        _controller = this.GetComponent<CharacterController>();
        run = false;
        move = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.LeftShift)) // 달리기 활성화
            run = true;
        else
            run = false; // 달리기 비활성화

        InputDodge();
        InputMovement();
        InputAttack();

    }

    void InputMovement()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
            return;

        keyCheck[0] = false;
        keyCheck[1] = false;
        keyCheck[2] = false;
        keyCheck[3] = false;

        finalSpeed = (run) ? runSpeed : speed;


        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 MoveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");


        if (Input.GetKey(KeyCode.W))
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1)); // 플레이어의 z, x 바꿔주기
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
            keyCheck[0] = true;
            _controller.Move(forward * finalSpeed * Time.deltaTime);

        }

        if (Input.GetKey(KeyCode.S))
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward * -1, new Vector3(1, 0, 1)); // 플레이어의 z, x 바꿔주기
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
            keyCheck[1] = true;
            _controller.Move(forward * finalSpeed * Time.deltaTime);


        }


        if (Input.GetKey(KeyCode.A))
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.right * -1, new Vector3(1, 0, 1)); // 플레이어의 z, x 바꿔주기
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
            keyCheck[2] = true;

            if(keyCheck[0] == true || keyCheck[1] == true)
                _controller.Move(forward * finalSpeed * Time.deltaTime * 0.025f);
            else
                _controller.Move(forward * finalSpeed * Time.deltaTime);



        }

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.right, new Vector3(1, 0, 1)); // 플레이어의 z, x 바꿔주기
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
            keyCheck[3] = true;

            if (keyCheck[0] == true || keyCheck[1] == true)
                _controller.Move(forward * finalSpeed * Time.deltaTime * 0.025f);
            else
                _controller.Move(forward * finalSpeed * Time.deltaTime);

        }




        float percent = ((run) ? 1 : 0.5f) * MoveDirection.normalized.magnitude;
        _animator.SetFloat("move", percent, 0.1f, Time.deltaTime);
    }

    void InputAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _animator.SetTrigger("onWeaponAttack");
        }
    }

    void InputDodge()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetTrigger("doDodge");
        }
    }
}
