using System.Collections;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    public MonsterInfo monsterInfo;
    int hp = 5;
    private enum State
    {
        Idle,
        Chase,
        Attack,
        Damaged,
    }
    IdleState idleState;
    GoblinChaseState goblineChaseState;
    AttackState attackState;
    DamagedState damagedState;
    Vector3 playerPos;
    [SerializeField]
    private State _curState;
    private MonsterStateMachine _monsterStateMachine;
    [SerializeField]
    float targetPos;
    public Transform platform;
    RaycastHit2D hitRight;
    RaycastHit2D hitLeft;
    public Goblin goblin;
    float firstPos;
    bool goblinGreet = false;
    int randomNum = 0;
    static bool detection = false;
    float aggroTime = 3f;
    float aggroInterval = 0f;

    private void Awake()
    {
        targetPos = transform.position.x;
        firstPos = targetPos;
    }
    private void Start()
    {
        Init();
        _curState = State.Idle;
        _monsterStateMachine = new MonsterStateMachine(idleState);
        StartCoroutine(Goblin_State());
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, monsterInfo.fieldOfView);
    }
    private void Update()
    {

    }

    IEnumerator Goblin_State()
    {
        while (true)
        {
            playerPos = GameObject.Find("Player").transform.position;
            switch (_curState)
            {
                case State.Idle:
                    hitRight = Physics2D.Raycast(transform.position, Vector2.right, 1.5f, 1 << LayerMask.NameToLayer("Platform"));
                    hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 1.5f, 1 << LayerMask.NameToLayer("Platform"));

                    if (goblinGreet)
                    {
                        GetComponent<Rigidbody2D>().AddForce(Vector3.up * 100);
                        yield return new WaitForSeconds(0.5f);
                        GetComponent<Rigidbody2D>().AddForce(Vector3.up * 100);
                        yield return new WaitForSeconds(0.5f);
                        targetPos = firstPos;
                        goblinGreet = false;
                    }
                    if (Vector3.Distance(playerPos, transform.position) <= monsterInfo.fieldOfView &&
                    Mathf.Abs(transform.position.y - playerPos.y) < 2)
                    {
                        transform.GetChild(0).gameObject.SetActive(true);
                        yield return new WaitForSeconds(1.5f);
                        transform.GetChild(0).gameObject.SetActive(false);
                        detection = true;
                        aggroInterval = Time.time;
                        ChangeState(State.Chase);
                    }
                    if (detection)
                        ChangeState(State.Chase);
                    if (Mathf.Abs(targetPos - transform.position.x) < 1)
                    {// 목표지점 도착
                        do
                            randomNum = Random.Range(-10, 11);
                        while (Mathf.Abs(randomNum) < 4);
                        targetPos = targetPos + randomNum;
                        yield return new WaitForSeconds(2f);
                    }
                    else if (Mathf.Abs(platform.position.x - platform.localScale.x / 2 - transform.position.x) < 1)
                    {// 왼쪽 모서리
                        randomNum = Random.Range(5, 11);
                        targetPos = targetPos + randomNum;
                        yield return new WaitForSeconds(2f);
                    }
                    else if (Mathf.Abs(platform.position.x + platform.localScale.x / 2 - transform.position.x) < 1)
                    {// 오른쪽 모서리
                        randomNum = Random.Range(-5, -11);
                        targetPos = targetPos + randomNum;
                        yield return new WaitForSeconds(2f);
                    }
                    else if (hitRight)
                    {// 오른쪽 벽
                        targetPos = targetPos + Random.Range(-5, -11);
                        yield return new WaitForSeconds(2f);
                    }
                    else if (hitLeft)
                    {// 왼쪽 벽
                        targetPos = targetPos + Random.Range(5, 11);
                        yield return new WaitForSeconds(2f);
                    }
                    transform.position += new Vector3(targetPos - transform.position.x, 0, 0).normalized * 0.08f;
                    break;

                case State.Chase:
                    if (Vector3.Distance(playerPos, transform.position) <= monsterInfo.attackRange)
                        ChangeState(State.Attack);
                    if (Vector3.Distance(playerPos, transform.position) > monsterInfo.fieldOfView)
                    {
                        if (Time.time - aggroInterval >= aggroTime)
                        {
                            aggroInterval = Time.time;
                            detection = false;
                            ChangeState(State.Idle);
                        }
                    }
                    else
                        aggroInterval = Time.time;

                    if (platform.position.x - platform.localScale.x / 2 >= transform.position.x)
                    {
                        ChangeState(State.Idle);
                    }
                    break;
                case State.Attack:
                    if (Vector3.Distance(playerPos, transform.position) > monsterInfo.attackRange)
                        ChangeState(State.Chase);
                    break;
                case State.Damaged:

                    break;
            }
            _monsterStateMachine.UPdateStage();

            yield return new WaitForSeconds(0.03f);
        }
    }

    private void Init()
    {
        idleState = new IdleState(gameObject);
        goblineChaseState = new GoblinChaseState(gameObject);
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
                _monsterStateMachine.ChangeState(goblineChaseState);
                break;
            case State.Attack:
                _monsterStateMachine.ChangeState(attackState);
                break;
            case State.Damaged:
                _monsterStateMachine.ChangeState(damagedState);
                break;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Goblin"))
        {
            goblinGreet = true;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Goblin"));
        }
    }
}
