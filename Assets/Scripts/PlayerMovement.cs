using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    bool shoot = false;
    float shootTime = 0.9f;
    float dur = 0f;


    public float hp = 100f;
    public Image hpBar;

    public GameObject sword;

    bool attacktrigger = false;



    // Start is called before the first frame update
    void Start()
    {
        dealState = DealState.DEALTIME;

        _animator = this.GetComponent<Animator>();
        _camera = Camera.main;
        _controller = this.GetComponent<CharacterController>();
        run = true;
        move = 0f;

        DisableSlash(0);
        DisableSlash(1);

    }

    // Update is called once per frame
    void Update()
    {
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        

        attacktrigger = false;

        if (movementState == MovementState.ATTACK)
            attacktrigger = true;


        Debug.Log(attacktrigger);

        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            movementState = MovementState.IDLE;


        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("charging") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        {
            _animator.SetInteger("charging", 2);
            shoot = true;
            //if (shootOnce)
            //{
            //    this.gameObject.GetComponent<GroundSlashShooter>().ShootProjectile();
            //    shootOnce = false;
            //}
        }

        if(shoot)
        {
            dur += Time.deltaTime;

            if(dur >= shootTime)
            {
                this.gameObject.GetComponent<GroundSlashShooter>().ShootProjectile();
                shoot = false;
                dur = 0f;
                
            }
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("charging_attack") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        {
            _animator.SetInteger("charging", 0);
        }

        InputDodge();
        InputMovement();
        InputAttack();

    }

    public void GetDamage(float damage)
    {
        hp -= damage;
        hpBar.fillAmount = hp / 100f;
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
            StartCoroutine(SlashAttack());
        }

        if(Input.GetMouseButtonDown(1))
        {
            _animator.SetInteger("charging", 1);
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
        //for (int i = 0; i < slashes.Count; ++i)
        //{
        //    yield return new WaitForSeconds(slashes[i].delay);
        //    slashes[i].slashObject.SetActive(true);
        //}

        if(attacktrigger == false)
        {
            yield return new WaitForSeconds(slashes[0].delay);
            slashes[0].slashObject.SetActive(true);
            yield return new WaitForSeconds(1);
            DisableSlash(0);
        }

        if(attacktrigger == true)
        {
            yield return new WaitForSeconds(slashes[1].delay);
            slashes[1].slashObject.SetActive(true);
            yield return new WaitForSeconds(1);
            DisableSlash(1);
        }

        //yield return new WaitForSeconds(1);
        //DisableSlash();

    }
    void DisableSlash(int index)
    {
        slashes[index].slashObject.SetActive(false);
        //for (int i = 0; i < slashes.Count; ++i)
        //{
        //    slashes[i].slashObject.SetActive(false);
        //}
    }


}
