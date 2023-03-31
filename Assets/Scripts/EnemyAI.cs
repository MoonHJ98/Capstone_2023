using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    GameObject player;
    Rigidbody rigidBody;
    public Animator _animator;
    enum State { Sleep, Idle, Run, Punch, PunchTwice, Rush, SideRoll, LeashAttack, StateEnd };
    State state;



    public float scanDistance;
    public float attackDistance;
    public float currentDistance;

    public float moveSpeed;
    public float rotSpeed;



    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 5f;
        rotSpeed = 3f;
        rigidBody = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        state = State.Idle;

        scanDistance = 20f;
        attackDistance = 5f;

    }

    // Update is called once per frame
    void Update()
    {

        
        currentDistance = (player.transform.position - transform.position).magnitude;


        UpdateState();
    }

    private void UpdateState()
    {
        switch (state)
        {
            case State.Sleep:
                UpdateSleep();
                break;
            case State.Idle:
                UpdateIdle();
                break;
            case State.Run:
                UpdateRun();
                break;
            case State.Punch:
                UpdatePunch();
                break;
            case State.PunchTwice:
                UpdatePunchTwice();
                break;
            case State.Rush:
                UpdateRush();
                break;
            case State.SideRoll:
                UpdateSideRoll();
                break;
            case State.LeashAttack:
                UpdateLeashAttack();
                break;
            case State.StateEnd:
                break;
            default:
                break;
        }
        return;
    }    
    private void UpdateSleep()
    {
        return;
    }
    private void UpdateIdle()
    {
        Debug.Log("Idle");

        currentDistance = (player.transform.position - transform.position).magnitude;

        _animator.SetInteger("move", (int)State.Idle);

        if (currentDistance <= scanDistance && currentDistance > attackDistance)
        {
            state = State.Run;
        }
        return;
    }
    private void UpdateRun()
    {
        Debug.Log("Run");
        _animator.SetInteger("move", (int)State.Run);

        //������Ʈ�� �̵�����
        Vector3 direction = player.transform.position - transform.position;
        //ȸ������(���ʹϾ�) ����
        Quaternion rot = Quaternion.LookRotation(direction);
        //���鼱������ �Լ��� �ε巯�� ȸ��ó��
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotSpeed); // 1f = ȸ���ӵ�

        transform.position += direction.normalized * Time.deltaTime * moveSpeed;
        
        currentDistance = direction.magnitude;

        if (currentDistance <= attackDistance)
        { 
            int randomState = Random.Range(0, 1); // ����
            switch (randomState)
            {
                case 0:
                    state = State.Punch;
                    break;
            }
        }
        return;
    }
    private void UpdatePunch()
    {
        Debug.Log("Punch");

        _animator.SetInteger("move", (int)State.Punch);

        currentDistance = (player.transform.position - transform.position).magnitude;

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Punch") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            state = State.Idle;
            return;
        }


        return;
    }
    private void UpdatePunchTwice()
    {

        /*
        if(animation �� && currentTierdPoint >= maxTierdPoint)
        {
            state = tired;
            checkOnce = true;
        }
        */
        return;
    }
    private void UpdateRush()
    {

        _animator.SetInteger("move", (int)State.Rush);
        //������Ʈ�� �̵�����
        Vector3 direction = player.transform.position - transform.position;
        //ȸ������(���ʹϾ�) ����
        Quaternion rot = Quaternion.LookRotation(direction);
        //���鼱������ �Լ��� �ε巯�� ȸ��ó��
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 1f); // 1f = ȸ���ӵ�

        
        /*
        if(animation �� && currentTierdPoint >= maxTierdPoint)
        {
            state = tired;
            checkOnce = true;
        }
        */
        return;

    }
    private void UpdateSideRoll()
    {

        /*
        if(animation �� && currentTierdPoint >= maxTierdPoint)
        {
            state = tired;
            checkOnce = true;
        }
        */
        return;
    }
    private void UpdateLeashAttack()
    {



        /*
        if(animation �� && currentTierdPoint >= maxTierdPoint)
        {
            state = tired;
            checkOnce = true;
        }
        */
        return;
    }


}
