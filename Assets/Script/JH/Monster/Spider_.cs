using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider_ : Monster
{
    public MonsterInfo monsterInfo;
    public int hp = 5;
    private enum State
    {
        Idle,
        Chase,
        Attack,
        Damaged
    }
    IdleState idleState;
    ChaseState chaseState;
    AttackState attackState;
    DamagedState damagedState;
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
            case State.Damaged:
                if (!GetComponent<Animator>().GetBool("onDamaged"))
                {
                    if (Vector3.Distance(transform.position, playerPos) > monsterInfo.attackRange)
                        ChangeState(State.Chase);
                    else if (Vector3.Distance(transform.position, playerPos) < monsterInfo.attackRange)
                        ChangeState(State.Attack);
                }
                break;
        }
        _monsterStateMachine.UPdateStage();
    }

    private void Init()
    {
        idleState = new IdleState(gameObject);
        chaseState = new ChaseState(gameObject);
        attackState = new AttackState(gameObject);
        damagedState = new DamagedState(gameObject);

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
            case State.Damaged:
                _monsterStateMachine.ChangeState(damagedState);
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeState(State.Damaged);
            Debug.Log(1);
            hp--;
        }
        Debug.Log(2);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _curState = State.Attack;
        }
    }
}
