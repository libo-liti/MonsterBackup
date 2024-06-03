using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Monster
{
    public MonsterInfo monsterInfo;
    int hp = 5;
    private enum State
    {
        Idle,
        Chase,
        Attack,
        Damaged,
        Move,
        Init,
        MoveBack
    }
    IdleState idleState;
    ChaseState chaseState;
    AttackState attackState;
    DamagedState damagedState;
    public Transform platform;
    Vector3[] platformConers;
    int currentConer;
    [SerializeField]
    bool isRight;
    public float timeOffPlatform = 0;
    public bool isActive;
    Vector3 playerPos;
    Vector3 platformSize;
    Vector3 platformPosition;
    [SerializeField]
    private State _curState;
    private MonsterStateMachine _monsterStateMachine;

    private void Awake()
    {
        platformSize = platform.localScale;
        platformPosition = platform.position;

        platformConers = new Vector3[4];
        platformConers[0] = platformPosition + new Vector3(-platformSize.x / 2.0f, -platformSize.y / 2.0f, 0);
        platformConers[1] = platformPosition + new Vector3(platformSize.x / 2.0f, -platformSize.y / 2.0f, 0);
        platformConers[2] = platformPosition + new Vector3(platformSize.x / 2.0f, platformSize.y / 2.0f, 0);
        platformConers[3] = platformPosition + new Vector3(-platformSize.x / 2.0f, platformSize.y / 2.0f, 0);

        transform.position = new Vector3(transform.position.x, platformConers[0].y, 0);

        if (transform.position.x >= platform.position.x)
        {
            isRight = true;
            currentConer = 1;
            transform.position = platformConers[1] + new Vector3(-platformSize.x / 4.0f, 0, 0);
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            isRight = false;
            currentConer = 0;
            transform.position = platformConers[0] + new Vector3(platformSize.x / 4.0f, 0, 0);
            transform.localScale = new Vector3(-1, -1, 1);
        }
    }
    private void Start()
    {
        Init();
        _curState = State.Init;
        _monsterStateMachine = new MonsterStateMachine(idleState);
    }

    private void Update()
    {
        playerPos = GameObject.Find("Player").transform.position;
        switch (_curState)
        {
            case State.Init:
                if (isActive)
                {
                    ChangeState(State.Move);
                    StartCoroutine(Transparency(gameObject));
                }
                break;
            case State.Move:
                Move();
                if ((!isRight && currentConer == 2) || (isRight && currentConer == 3))
                    ChangeState(State.Chase);
                break;
            case State.MoveBack:
                MoveBack();

                break;
            case State.Idle:
                if (isActive)
                    ChangeState(State.Chase);
                else
                {
                    aggroFree();
                }
                break;
            case State.Chase:
                if (transform.position.x > platform.position.x + platformSize.x / 2.0f ||
                transform.position.x < platform.position.x - platformSize.x / 2.0f)
                    ChangeState(State.Idle);
                else if (Vector3.Distance(transform.position, playerPos) < monsterInfo.attackRange)
                    ChangeState(State.Attack);

                if (!isActive)
                    aggroFree();
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
        if (_curState != State.Move || _curState != State.Init)
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
            hp--;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _curState = State.Attack;
        }
    }
    private void Move()
    {
        if (isRight)
        {
            if (Vector3.Distance(transform.position, platformConers[currentConer]) < 0.1f)
            {
                currentConer++;
                switch (currentConer)
                {
                    case 2:
                        transform.Rotate(0, 0, 90);
                        break;
                    case 3:
                        transform.Rotate(0, 0, -90);
                        break;
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, platformConers[currentConer]) < 0.1f)
            {
                currentConer--;
                if (currentConer < 0)
                    currentConer = 3;
                switch (currentConer)
                {
                    case 3:
                        transform.Rotate(0, 0, -90);
                        break;
                    case 2:
                        transform.Rotate(0, 0, 90);
                        break;
                }
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, platformConers[currentConer], 4 * Time.deltaTime);
    }
    void aggroFree()
    {
        timeOffPlatform += Time.deltaTime;
        if (timeOffPlatform > 3.0f)
        {
            if (_curState != State.Idle)
                ChangeState(State.Idle);
            else
            {
                ChangeState(State.MoveBack);
                if (isRight)
                {
                    currentConer = 2;
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    currentConer = 3;
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
    }
    void MoveBack()
    {
        if (isRight)
        {
            if (Vector3.Distance(transform.position, platformConers[currentConer]) < 0.1f)
            {
                currentConer--;
                StartCoroutine(Transparency(gameObject));
                switch (currentConer)
                {
                    case 1:
                        transform.Rotate(0, 0, -90);
                        break;
                    case 0:
                        transform.Rotate(0, 0, -90);
                        break;
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, platformConers[currentConer]) < 0.1f)
            {
                currentConer = ++currentConer % 4;
                StartCoroutine(Transparency(gameObject));
                switch (currentConer)
                {
                    case 0:
                        transform.Rotate(0, 0, 90);
                        break;
                    case 1:
                        transform.Rotate(0, 0, 90);
                        break;
                }
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, platformConers[currentConer], 4 * Time.deltaTime);
    }
    IEnumerator Transparency(GameObject obj)
    {
        Color color = obj.GetComponent<SpriteRenderer>().color;
        if (color.a == 0)
        {
            while (color.a <= 1)
            {
                color.a += 0.03f;
                obj.GetComponent<SpriteRenderer>().color = color;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (color.a > 0)
        {
            while (color.a >= 0)
            {
                color.a -= 0.03f;
                obj.GetComponent<SpriteRenderer>().color = color;
                yield return new WaitForSeconds(0.1f);
            }
            ChangeState(State.Init);
        }
    }
}
