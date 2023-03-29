using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    GameObject player;
    public Animator _animator;
    enum State { Sleep, Idle, Run, Punch, PunchTwice, Rush, SideRoll, LeashAttack, StateEnd }
    State state;

    public float scanDistance;
    public float attackDistance;
    public float currentDistance;

    public int currentTierdPoint;
    public int maxTieldPoint;

    public int currentAttackPoint;
    public int maxAttackPoint;

    private bool checkOnce;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        state = State.Idle;

        scanDistance = 20f;
        attackDistance = 5f;

        currentTierdPoint = 0;
        maxTieldPoint = 10;


        checkOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        currentDistance = (player.transform.position - transform.position).magnitude;

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
                UpdatePunchTwice();
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

    }

    private void UpdateSleep()
    {
        return;
    }
    private void UpdateIdle()
    {

        currentDistance = (player.transform.position - transform.position).magnitude;
        Debug.Log("Idle");
        _animator.SetInteger("move", (int)State.Idle);

        if (currentDistance <= scanDistance && currentDistance > attackDistance)
        {
            state = State.Run;
            //int randomState = Random.Range(0, 1);
            //switch (randomState)
            //{
            //    case 0:
            //        
            //        break;
            //    case 1:
            //        state = State.Rush;
            //        break;
            //}


        }
        return;
    }
    private void UpdateRun()
    {

        _animator.SetInteger("move", (int)State.Run);


        navMeshAgent.SetDestination(player.transform.position);

        Debug.Log("Run");

        currentDistance = (player.transform.position - transform.position).magnitude;
        if (currentDistance <= attackDistance)
        {
            state = State.Idle;
            //int randomState = Random.Range(0, 1);
            //switch (randomState)
            //{
            //    case 0:
            //        state = State.Punch;
            //        navMeshAgent.SetDestination(transform.position);
            //        break;
            //    case 1:
            //        state = State.PunchTwice;
            //        break;
            //}
        }
        return;
    }
    private void UpdatePunch()
    {
        if (checkOnce)
        {
            currentTierdPoint += 1;
            checkOnce = false;
        }
        _animator.SetTrigger("Punch");


        /*
        if(animation 场 && currentTierdPoint >= maxTierdPoint)
        {
            state = tired;
            checkOnce = true;
        }
        */
        return;
    }
    private void UpdatePunchTwice()
    {
        if (checkOnce)
        {
            currentTierdPoint += 2;
            checkOnce = false;
        }

        /*
        if(animation 场 && currentTierdPoint >= maxTierdPoint)
        {
            state = tired;
            checkOnce = true;
        }
        */
        return;
    }
    private void Rush()
    {
        if (checkOnce)
        {
            currentTierdPoint += 4;
            checkOnce = false;
        }
        /*
        if(animation 场 && currentTierdPoint >= maxTierdPoint)
        {
            state = tired;
            checkOnce = true;
        }
        */
        return;

    }
    private void UpdateSideRoll()
    {
        if (checkOnce)
        {
            currentTierdPoint += 4;
            checkOnce = false;
        }

        /*
        if(animation 场 && currentTierdPoint >= maxTierdPoint)
        {
            state = tired;
            checkOnce = true;
        }
        */
        return;
    }
    private void UpdateLeashAttack()
    {
        if (checkOnce)
        {
            currentTierdPoint += 4;
            checkOnce = false;
        }
        /*
        if(animation 场 && currentTierdPoint >= maxTierdPoint)
        {
            state = tired;
            checkOnce = true;
        }
        */
        return;
    }


}
