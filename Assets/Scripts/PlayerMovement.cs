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
        if (Input.GetKey(KeyCode.LeftControl))
            toggleCameraRotation = true; // 둘러보기 활성화
        else
            toggleCameraRotation = false; // 둘러보기 비활성화

        if (Input.GetKey(KeyCode.LeftShift)) // 달리기 활성화
            run = true;
        else
            run = false; // 달리기 비활성화


        ToggleCamaraRotation();
        InputMovement();
    }

    void InputMovement()
    {
        finalSpeed = (run) ? runSpeed : speed;


        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 MoveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.W))
            _controller.Move(forward * finalSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.S))
            _controller.Move(forward * -finalSpeed * Time.deltaTime);
     

        if (Input.GetKey(KeyCode.A))
            _controller.Move(right * -finalSpeed * Time.deltaTime);
      
        if (Input.GetKey(KeyCode.D))
            _controller.Move(right * finalSpeed * Time.deltaTime);
  

        float percent = ((run) ? 1 : 0.5f) * MoveDirection.normalized.magnitude;
        _animator.SetFloat("move", percent,0.1f, Time.deltaTime);
    }
    void ToggleCamaraRotation()
    {
        if (toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1)); // 플레이어의 z, x 바꿔주기
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }
}
