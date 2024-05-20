using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    public MonsterInfo monsterInfo;
    private enum State
    {
        Idle,
        Chase,
        Attack
    }
    IdleState idleState;
    ChaseState chaseState;
    AttackState attackState;
    Vector3 playerPos;
    [SerializeField]
    private State _curState;
    private MonsterStateMachine _monsterStateMachine;

    private void Start()
    {
        Init();
        _curState = State.Idle;
        _monsterStateMachine = new MonsterStateMachine(new IdleState(gameObject));
    }

    private void Update()
    {
        playerPos = GameObject.Find("Player").transform.position;
        switch (_curState)
        {
            case State.Idle:
                if (Vector3.Distance(transform.position, playerPos) < monsterInfo.fieldOfView)
                    ChangeState(State.Chase);
                break;
            case State.Chase:
                if (Vector3.Distance(transform.position, playerPos) > monsterInfo.fieldOfView)
                    ChangeState(State.Idle);
                else if (Vector3.Distance(transform.position, playerPos) < monsterInfo.attackRange)
                    ChangeState(State.Attack);
                break;
            case State.Attack:
                if (Vector3.Distance(transform.position, playerPos) > monsterInfo.attackRange)
                    ChangeState(State.Chase);
                break;
        }
        _monsterStateMachine.UPdateStage();
    }

    private void Init()
    {
        idleState = new IdleState(gameObject);
        chaseState = new ChaseState(gameObject);
        attackState = new AttackState(gameObject);

        playerPos = Vector3.zero;
    }
    private void ChangeState(State nextState)
    {
        _curState = nextState;
        switch (_curState)
        {
            case State.Idle:
                _monsterStateMachine.ChangeState(idleState);
                break;
            case State.Chase:
                _monsterStateMachine.ChangeState(chaseState);
                break;
            case State.Attack:
                _monsterStateMachine.ChangeState(attackState);
                break;
        }
    }
}
