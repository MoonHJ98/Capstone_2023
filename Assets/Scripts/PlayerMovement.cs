using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState {IDLE, MOVE, ATTACK, DODGE};
    public enum DealState { NORMAL, DEALTIME};

    [System.Serializable]
    public class Slash
    {
        public GameObject slashObject;
        public float delay;
    }

    public MovementState movementState = MovementState.MOVE;
    public DealState dealState;

    Animator _animator;
    Camera _camera;
    CharacterController _controller;



    public float speed = 5f;
    public float runSpeed = 8f;
    public float finalSpeed;
    public float dodgeSpeed = 5f;

    public bool run;

    public float move;

    public float smoothness = 7f;

    public bool toggleCameraRotation;

    public Vector3 DodgeDir;


    private bool[] keyCheck = new bool[4];

    public List<Slash> slashes;

    // Start is called before the first frame update
    void Start()
    {
        dealState = DealState.DEALTIME;

        _animator = this.GetComponent<Animator>();
        _camera = Camera.main;
        _controller = this.GetComponent<CharacterController>();
        run = true;
        move = 0f;

        DisableSlash();
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InputDodge();
        InputMovement();
        InputAttack();


    }

    void InputMovement()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
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
            movementState = MovementState.MOVE;

        }

        if (Input.GetKey(KeyCode.S))
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward * -1, new Vector3(1, 0, 1)); // 플레이어의 z, x 바꿔주기
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
            keyCheck[1] = true;
            _controller.Move(forward * finalSpeed * Time.deltaTime);
            movementState = MovementState.MOVE;


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

            movementState = MovementState.MOVE;


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
            movementState = MovementState.MOVE;

        }




        float percent = ((run) ? 1 : 0.5f) * MoveDirection.normalized.magnitude;

        //Debug.Log(percent);

        _animator.SetFloat("move", percent, 0.1f, Time.deltaTime);
    }

    void InputAttack()
    {
        //if (movementState == MovementState.DODGE)
        //    return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            movementState = MovementState.ATTACK;

            _animator.SetTrigger("onWeaponAttack");
            if (dealState == DealState.DEALTIME)
            {
                StartCoroutine(SlashAttack());
            }
        }
    }

        void InputDodge()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
        {
            _controller.Move(DodgeDir * dodgeSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(DodgeDir), Time.deltaTime * smoothness);

            movementState = MovementState.DODGE;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetTrigger("doDodge");
            movementState = MovementState.DODGE;
           
            if(Input.GetKey(KeyCode.A))
            {
                DodgeDir = _camera.transform.right.normalized * -1;
                if(Input.GetKey(KeyCode.W))
                {
                    DodgeDir += _camera.transform.forward.normalized;
                    DodgeDir.y = 0;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    DodgeDir += _camera.transform.forward.normalized * -1;
                    DodgeDir.y = 0;
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                DodgeDir = _camera.transform.right.normalized;
                if (Input.GetKey(KeyCode.W))
                {
                    DodgeDir += _camera.transform.forward.normalized;
                    DodgeDir.y = 0;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    DodgeDir += _camera.transform.forward.normalized * -1;
                    DodgeDir.y = 0;
                }

            }
            else if (Input.GetKey(KeyCode.W))
            {
                DodgeDir = _camera.transform.forward.normalized;
                DodgeDir.y = 0;

            }
            else if (Input.GetKey(KeyCode.S))
            {
                DodgeDir = _camera.transform.forward.normalized * -1;
                DodgeDir.y = 0;

            }

        }
    }

    IEnumerator SlashAttack()
    {
        for (int i = 0; i < slashes.Count; ++i)
        {
            yield return new WaitForSeconds(slashes[i].delay);
            slashes[i].slashObject.SetActive(true);
        }

        yield return new WaitForSeconds(1);
        DisableSlash();

    }
    void DisableSlash()
    {
        for(int i = 0; i < slashes.Count; ++i)
        {
            slashes[i].slashObject.SetActive(false);
        }
    }
}
