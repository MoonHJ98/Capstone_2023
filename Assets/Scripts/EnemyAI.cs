using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    GameObject player;
    NavMeshAgent navMeshAgent;
    CharacterController characterController;
    public Animator _animator;
    enum State { Patroll, Detect, Attack, StateEnd };
    enum AniState { Idle, Walk, Jump, Punch_1, Punch_2, AniStateEnd };
    State state;

    public List<Transform> patrollPoints;
    private List<Transform> patrollPointsCopy;
    private Transform targetTransform;
    private bool checkOnce;

    public float jumpPower;




    public float scanDistance;
    public float attackDistance;
    public float currentDistance;

    public float moveSpeed;
    public float rotSpeed;

    float yVelocity;
    float zVelocity;
    Vector3 dir;

    public float gravity;
    public float jumpSpeed;

    public float angle;


    // Start is called before the first frame update
    void Start()
    {
        jumpSpeed = 5f;
        gravity = -20f;
        jumpPower = 5f;
        moveSpeed = 5f;
        rotSpeed = 3f;
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
    }


    private void UpdateState()
    {
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
            state = State.Attack;
            navMeshAgent.SetDestination(transform.position);

        }



    }

    private void UpdateAttack()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            checkOnce = true;
            state = State.Patroll;
            navMeshAgent.SetDestination(transform.position);

            patrollPointsCopy.Clear();

            for (int i = 0; i < patrollPoints.Count; ++i)
                patrollPointsCopy.Add(patrollPoints[i]);

            return;
        }

        _animator.SetInteger("Move", (int)AniState.Punch_1);

        Vector3 _dir = (player.transform.position - transform.position).normalized;
        // 방향을 바라보는 Quaternion을 구한다.
        Quaternion _rot = Quaternion.LookRotation(_dir);


        Vector3 v = (player.transform.position - transform.position) - transform.forward * -1f;

        angle = Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg + 180f;

        //if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
        //{
        //    checkOnce = true;
        //    state = State.Patroll;
        //    navMeshAgent.SetDestination(transform.position);
        //
        //    patrollPointsCopy.Clear();
        //
        //    for (int i = 0; i < patrollPoints.Count; ++i)
        //        patrollPointsCopy.Add(patrollPoints[i]);
        //
        //    return;
        //}


    }
}
