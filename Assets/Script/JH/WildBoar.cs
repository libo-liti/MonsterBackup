using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildBoar : MonoBehaviour
{
    public MonsterInfo monsterInfo;
    int hp = 5;
    private enum State
    {
        Idle,
        Chase,
        Damaged,
    }
    IdleState idleState;
    WildBoarChaseState wildBoarChaseState;
    DamagedState damagedState;
    RaycastHit2D hitRight;
    RaycastHit2D hitLeft;
    Vector3 playerPos;
    [SerializeField]
    private State _curState;
    private MonsterStateMachine _monsterStateMachine;

    private void Awake()
    {

    }
    private void Start()
    {
        Init();
        _curState = State.Idle;
        _monsterStateMachine = new MonsterStateMachine(idleState);
        StartCoroutine(WildBoar_State());
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, monsterInfo.fieldOfView);
    }
    private void Update()
    {

    }

    IEnumerator WildBoar_State()
    {
        while (true)
        {
            playerPos = GameObject.Find("Player").transform.position;
            switch (_curState)
            {
                case State.Idle:
                    if (Vector3.Distance(playerPos, transform.position) <= monsterInfo.fieldOfView &&
                    Mathf.Abs(transform.position.y - playerPos.y) < 2)
                        ChangeState(State.Chase);
                    break;
                case State.Chase:
                    hitRight = Physics2D.Raycast(transform.position, Vector2.right, 1.5f, 1 << LayerMask.NameToLayer("Platform"));
                    hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 1.5f, 1 << LayerMask.NameToLayer("Platform"));
                    if (hitRight || hitLeft)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                            yield return new WaitForSeconds(0.5f);
                            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                            yield return new WaitForSeconds(0.5f);
                        }
                        ChangeState(State.Idle);
                    }
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
        wildBoarChaseState = new WildBoarChaseState(gameObject);
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
                _monsterStateMachine.ChangeState(wildBoarChaseState);
                break;
            case State.Damaged:
                _monsterStateMachine.ChangeState(damagedState);
                break;
        }
    }
}
