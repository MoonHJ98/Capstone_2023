using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    GameObject player;
    NavMeshAgent navMeshAgent;
    CharacterController characterController;
    public Animator _animator;

    public float hp = 100f;
    public Image hpBar;
    enum State { Patroll, Detect, Attack, StateEnd };
    enum AniState { Idle, Walk, Jump, Punch_1, Punch_2, Run, JumpAttack, SideRoll, LeashAttack, AniStateEnd };

    /*
        가까이 있을 때
        Front_1 : 앞발 공격 -> 점프 후 뒤로 -> 점프 내려찍기
        Front_2 : 앞발 두번 공격 -> 앞발 공격
        
        Side_1 : 구르기 -> 점프, 회전으로 방향 맞추기-> 앞발 공격
        Side_2 : 구르기 -> 목줄 공격
        
        Back_1 : 점프, 회전으로 방향 맞추기 -> 앞발 공격
        Back_2 : 점프, 회전으로 방향 맞추기 -> 점프 공격
        
        멀리 있을 때
        달리기
        점프 공격

        앞 230~310
        오른쪽 130 230
        뒤 50 ~ 130
        왼쪽 0 ~ 50, 310 ~ 360
     */


    enum AttackPattern { Front_1, Front_2, Side_1, Side_2, Back_1, Back_2, AttackPatternEnd }
    State state;
    AttackPattern attackPattern;
    AniState aniState;

    public List<Transform> patrollPoints;
    private List<Transform> patrollPointsCopy;
    private Transform targetTransform;
    private bool checkOnce;

    public float jumpPower;

    private bool isJump;




    public float scanDistance;
    public float attackDistance;
    public float currentDistance;


    float yVelocity;
    float zVelocity;
    Vector3 dir;

    public float gravity;
    public float jumpSpeed;

    public float angle;

    private bool patternEnd;

    // 점프 공격 했을 때 맞는 거리
    float JumpAttackEnableDistance = 35f;




    // Start is called before the first frame update
    void Start()
    {
        patternEnd = true;
        jumpSpeed = 5f;
        gravity = -20f;
        jumpPower = 5f;

        isJump = false;
        attackPattern = AttackPattern.AttackPatternEnd;
        aniState = AniState.AniStateEnd;

        characterController = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        state = State.Patroll;

        scanDistance = 20f;
        attackDistance = 5f;

        patrollPointsCopy = new List<Transform>();

        for (int i = 0; i < patrollPoints.Count; ++i)
            patrollPointsCopy.Add(patrollPoints[i]);

        checkOnce = true;



    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();


        if (Input.GetKeyDown(KeyCode.C))
        {
            if (hp > 0f)
            {
                hp -= 10f;
                hpBar.fillAmount = hp / 100f;
            }
        }
    }



    private void UpdateState()
    {
        currentDistance = (player.transform.position - transform.position).magnitude;

        switch (state)
        {
            case State.Patroll:
                UpdatePatroll();
                break;
            case State.Detect:
                UpdateDetect();
                break;
            case State.Attack:
                UpdateAttack();
                break;
            case State.StateEnd:
                break;
            default:
                break;
        }
    }

    private void UpdatePatroll()
    {

        if (checkOnce)
        {
            int index = Random.Range(0, patrollPointsCopy.Count);
            targetTransform = patrollPointsCopy[index];

            patrollPointsCopy.RemoveAt(index);

            checkOnce = false;
        }

        _animator.SetInteger("Move", (int)AniState.Walk);
        aniState = AniState.Walk;


        navMeshAgent.SetDestination(targetTransform.position);

        float length = (targetTransform.position - transform.position).magnitude;

        if (Input.GetKeyDown(KeyCode.B))
        {
            checkOnce = true;
            state = State.Detect;
            navMeshAgent.SetDestination(transform.position);

            patrollPointsCopy.Clear();

            for (int i = 0; i < patrollPoints.Count; ++i)
                patrollPointsCopy.Add(patrollPoints[i]);

            return;
        }

        if (length <= 5f)
        {
            checkOnce = true;
            if (patrollPointsCopy.Count == 0)
            {
                for (int i = 0; i < patrollPoints.Count; ++i)
                    patrollPointsCopy.Add(patrollPoints[i]);
            }
        }

    }

    private void UpdateDetect()
    {
        _animator.SetInteger("Move", (int)AniState.Jump);
        aniState = AniState.Jump;


        Vector3 rotate = player.transform.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotate), Time.deltaTime * rotate.magnitude);


        player = GameObject.FindWithTag("Player");

        if (checkOnce)
        {
            yVelocity = jumpPower;

            dir = (player.transform.position - transform.position).normalized * -1f;
            checkOnce = false;
        }

        yVelocity += gravity * Time.deltaTime;

        dir.y = yVelocity;

        characterController.Move(dir * jumpSpeed * Time.deltaTime);

        if (characterController.collisionFlags == CollisionFlags.Below)
        {
            int a = 10;
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                state = State.Attack;
                navMeshAgent.SetDestination(transform.position);
                checkOnce = true;
            }

        }



    }

    private void UpdateAttack()
    {

        Vector3 _dir = (player.transform.position - transform.position).normalized;
        // 방향을 바라보는 Quaternion을 구한다.
        Quaternion _rot = Quaternion.LookRotation(_dir);


        Vector3 v = (player.transform.position - transform.position) - transform.forward * -1f;

        angle = Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg + 180f;

        //attackPattern = AttackPattern.Back_1;

        if (patternEnd == true)
        {
            /*
             앞 230~310
             오른쪽 130 230
             뒤 50 ~ 130
             왼쪽 0 ~ 50, 310 ~ 360
             */
        
            if(angle >= 270 && angle <= 350)
            {
                //앞
                //Debug.Log("앞");
                int pattern = Random.Range(0, 1);
                switch (pattern)
                {
                    case 0:
                        attackPattern = AttackPattern.Front_1;
                        break;
                    case 1:
                        attackPattern = AttackPattern.Front_2;
                        break;
                    default:
                        break;
                }
        
            }
            else if(angle > 170 && angle < 270)
            {
                //오른쪽
                //Debug.Log("오른쪽");
                int pattern = Random.Range(0, 1);
                switch (pattern)
                {
                    case 0:
                        attackPattern = AttackPattern.Side_1;
                        break;
                    case 1:
                        attackPattern = AttackPattern.Side_2;
                        break;
                    default:
                        break;
                }
        
            }
            else if(angle >= 90 && angle <= 170)
            {
                //뒤
                //Debug.Log("뒤");
                int pattern = Random.Range(0, 1);
                switch (pattern)
                {
                    case 0:
                        attackPattern = AttackPattern.Back_1;
                        break;
                    case 1:
                        attackPattern = AttackPattern.Back_2;
                        break;
                    default:
                        break;
                }
        
            }
            else if((angle >= 0 && angle <90) || (angle > 350 && angle < 360))
            {
                //왼쪽
                //Debug.Log("왼쪽");
                int pattern = Random.Range(0, 1);
                switch (pattern)
                {
                    case 0:
                        attackPattern = AttackPattern.Side_1;
                        break;
                    case 1:
                        attackPattern = AttackPattern.Side_2;
                        break;
                    default:
                        break;
                }
        
            }
        
            patternEnd = false;
        }

        UpdateAttackPattern();
    }

    private void UpdateAttackPattern()
    {

        switch (attackPattern)
        {
            case AttackPattern.Front_1:
                if (checkOnce)
                {
                    aniState = AniState.Punch_1;
                    checkOnce = false;
                }
                UpdateFront_1();
                break;
            case AttackPattern.Front_2:
                if (checkOnce)
                {
                    aniState = AniState.Punch_2;
                    checkOnce = false;
                }
                UpdateFront_2();
                break;
            case AttackPattern.Side_1:
                if (checkOnce)
                {
                    aniState = AniState.SideRoll;
                    checkOnce = false;
                }
                UpdateSide_1();
                break;
            case AttackPattern.Side_2:
                if (checkOnce)
                {
                    aniState = AniState.SideRoll;
                    checkOnce = false;
                }
                UpdateSide_2();
                break;
            case AttackPattern.Back_1:
                if (checkOnce)
                {
                    aniState = AniState.Jump;
                    checkOnce = false;
                }
                UpdateBack_1();
                break;
            case AttackPattern.Back_2:
                if (checkOnce)
                {
                    aniState = AniState.Jump;
                    checkOnce = false;
                }
                UpdateBack_2();
                break;
        }
    }

    private void UpdateFront_1()
    {
        switch (aniState)
        {
            case AniState.Punch_1:
                UpdatePunch_1(AniState.Jump);
                break;
            case AniState.Jump:
                UpdateJump(AniState.JumpAttack, 5f);
                break;
            case AniState.JumpAttack:
                UpdateJumpAttack(AniState.Punch_1, true);
                break;
            default:
                break;
        }
    }

    private void UpdateFront_2()
    {
        switch (aniState)
        {
            case AniState.Punch_2:
                UpdatePunch_2(AniState.Punch_1);
                break;
            case AniState.Punch_1:
                UpdatePunch_1(AniState.Punch_2, true);
                break;
            default:
                break;
        }
    }
    private void UpdateSide_1()
    {
        switch (aniState)
        {
            case AniState.SideRoll:
                UpdateSideRoll(AniState.Jump);
                break;
            case AniState.Jump:
                UpdateJump(AniState.Punch_1);
                break;
            case AniState.Punch_1:
                UpdatePunch_1(AniState.SideRoll, true);
                break;
            default:
                break;
        }
    }
    private void UpdateSide_2()
    {
        switch (aniState)
        {
            case AniState.SideRoll:
                UpdateSideRoll(AniState.LeashAttack);
                break;
            case AniState.LeashAttack:
                UpdateLeashAttack(AniState.SideRoll, true);
                break;
            default:
                break;
        }
    }
    private void UpdateBack_1()
    {
        switch (aniState)
        {
            case AniState.Jump:
                UpdateJump(AniState.Punch_1);
                break;
            case AniState.Punch_1:
                UpdatePunch_1(AniState.Jump, true);
                break;
            default:
                break;
        }
    }
    private void UpdateBack_2()
    {
        switch (aniState)
        {
            case AniState.Jump:
                UpdateJump(AniState.JumpAttack, 5f);
                break;
            case AniState.JumpAttack:
                UpdateJumpAttack(AniState.Jump, true);
                break;
            default:
                break;
        }
    }
    private void UpdateIdle(AniState _nextAniState)
    {

    }

    private void UpdateWalk(AniState _nextAniState)
    {

    }

    private void UpdateJump(AniState _nextAniState, float jumpDistance = 1f, bool _patternEnd = false)
    {
        _animator.SetInteger("Move", (int)AniState.Jump);

        player = GameObject.FindWithTag("Player");

        if (isJump == false)
        {
            yVelocity = jumpPower;


            dir = (player.transform.position - transform.position).normalized * -1f * jumpDistance;

            isJump = true;
            navMeshAgent.enabled = false;
            characterController.enabled = true;


        }


        Vector3 rotate = player.transform.position - transform.position;
        rotate.y = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotate), Time.deltaTime * rotate.magnitude);

        yVelocity += gravity * Time.deltaTime;

        dir.y = yVelocity;

        characterController.Move(dir * jumpSpeed * Time.deltaTime);

        if (_nextAniState == AniState.JumpAttack)
        {
            if (characterController.collisionFlags == CollisionFlags.Below)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(transform.position);
                characterController.enabled = false;

                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {

                    isJump = false;
                    aniState = _nextAniState;
                    targetTransform = player.transform;
                    characterController.enabled = true;

                }


                //float length = (targetTransform.position - transform.position).magnitude;

            }
        }
        else if (characterController.collisionFlags == CollisionFlags.Below)
        {
            //navMeshAgent.SetDestination(transform.position);
            if (_patternEnd)
                checkOnce = true;

            aniState = _nextAniState;
            isJump = false;

        }


    }

    private void UpdatePunch_1(AniState _nextAniState, bool _patternEnd = false)
    {
        _animator.SetInteger("Move", (int)AniState.Punch_1);

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Punch_1") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            aniState = _nextAniState;
            if (_patternEnd == true)
            {
                checkOnce = true;
                patternEnd = true;
            }
        }

    }
    private void UpdatePunch_2(AniState _nextAniState, bool _patternEnd = false)
    {
        _animator.SetInteger("Move", (int)AniState.Punch_2);

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Punch_2") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            aniState = _nextAniState;
            if (_patternEnd == true)
            {
                checkOnce = true;
                patternEnd = true;
            }
        }
    }
    private void UpdateJumpAttack(AniState _nextAniState, bool _patternEnd = false)
    {
        _animator.SetInteger("Move", (int)AniState.JumpAttack);


        if (isJump == false)
        {
            yVelocity = 10f;

            dir = (targetTransform.position - transform.position).normalized * 2.5f;

            navMeshAgent.enabled = false;
            isJump = true;
            characterController.enabled = true;


        }

        yVelocity += gravity * Time.deltaTime;

        dir.y = yVelocity;


        characterController.Move(dir * jumpSpeed * Time.deltaTime);


        if (characterController.collisionFlags == CollisionFlags.Below)
        {
            navMeshAgent.enabled = true;
            characterController.enabled = false;

            navMeshAgent.SetDestination(transform.position);

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                characterController.enabled = true;

                isJump = false;
                aniState = _nextAniState;
                if (_patternEnd == true)
                {
                    checkOnce = true;
                    patternEnd = true;
                }
            }
        }

    }

    private void UpdateSideRoll(AniState _nextAniState, bool _patternEnd = false)
    {
        _animator.SetInteger("Move", (int)AniState.SideRoll);


        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("SideRoll") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            aniState = _nextAniState;
            if (_patternEnd == true)
            {
                checkOnce = true;
                patternEnd = true;
            }
        }
    }

    private void UpdateLeashAttack(AniState _nextAniState, bool _patternEnd = false)
    {
        _animator.SetInteger("Move", (int)AniState.LeashAttack);


        //if (_animator.GetCurrentAnimatorStateInfo(0).IsName("LeashAttack") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        //{
        //    aniState = _nextAniState;
        //    if (_patternEnd == true)
        //    {
        //        checkOnce = true;
        //        patternEnd = true;
        //    }
        //}
    }
}
