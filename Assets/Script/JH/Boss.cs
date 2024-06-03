using System.Collections;
using System.Transactions;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private enum State
    {
        Waiting,
        FirstAction,
        Idle,
        Earthquake,
        Tornado,
        Dash,
        LongJump,
        Cry
    }
    State _curState;
    GameObject virtualCam2;
    public GameObject[] walls;
    Vector3 playerPos;
    Rigidbody2D rigid;
    CinemachineVirtualCamera cam;
    CinemachineBasicMultiChannelPerlin noise;
    private void Awake()
    {
        _curState = State.Waiting;
        rigid = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        cam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        StartCoroutine(Battle());
    }

    private void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (_curState == State.Cry)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 10);
        }
    }

    IEnumerator Battle()
    {
        while (true)
        {
            playerPos = GameObject.Find("Player").transform.position;

            if (playerPos.x - transform.position.x >= 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);

            switch (_curState)
            {
                case State.Waiting:
                    if (Vector3.Distance(playerPos, transform.position) < 20)
                    {
                        _curState = State.FirstAction;
                        virtualCam2 = GameObject.Find("Cam").transform.GetChild(0).gameObject;
                    }

                    break;
                case State.FirstAction:
                    virtualCam2.SetActive(true);
                    yield return new WaitForSeconds(3f);
                    walls[0].SetActive(true);
                    walls[1].SetActive(true);
                    yield return new WaitForSeconds(3f);
                    virtualCam2.SetActive(false);
                    walls[0].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    walls[1].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    yield return new WaitForSeconds(3f);
                    _curState = State.Idle;
                    break;
                case State.Idle:
                    if (Input.GetKeyDown(KeyCode.E))
                        _curState = State.Earthquake;
                    if (Input.GetKeyDown(KeyCode.T))
                        _curState = State.Tornado;
                    if (Input.GetKeyDown(KeyCode.D))
                        _curState = State.Dash;
                    if (Input.GetKeyDown(KeyCode.J))
                        _curState = State.LongJump;
                    if (Input.GetKeyDown(KeyCode.C))
                        _curState = State.Cry;
                    break;
                case State.Earthquake:
                    rigid.AddForce(Vector3.up * 400);
                    yield return new WaitUntil(() => rigid.velocity.y < -2);

                    rigid.gravityScale = 50f;

                    yield return new WaitUntil(() => rigid.velocity.y == 0);
                    noise.m_AmplitudeGain = 5;
                    noise.m_FrequencyGain = 1;

                    // if (GameObject.Find("Player").GetComponent<Rigidbody2D>().velocity.y == 0)
                    // {
                    //     PlayerMoveinFixedUpdate.moveFlag = false;
                    //     PlayerMoveinFixedUpdate.lastTime = Time.time;
                    //     PlayerMoveinFixedUpdate.lastPos = playerPos;
                    // }
                    yield return new WaitForSeconds(1f);

                    noise.m_AmplitudeGain = 0;
                    noise.m_FrequencyGain = 0;
                    rigid.gravityScale = 1;

                    _curState = State.Idle;
                    break;
                case State.Tornado:
                    transform.Rotate(0, 20, 0);
                    if (playerPos.x - transform.position.x >= 0)
                        transform.position += Vector3.right * 0.2f;
                    else
                        transform.position += Vector3.left * 0.2f;

                    if (Input.GetKeyDown(KeyCode.T))
                        _curState = State.Idle;
                    break;
                case State.Dash:
                    for (int i = 0; i < 2; i++)
                    {
                        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                        yield return new WaitForSeconds(0.2f);
                        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                        yield return new WaitForSeconds(0.2f);
                    }
                    yield return new WaitForSeconds(1f);

                    rigid.AddForce(new Vector3(playerPos.x - transform.position.x, 0, 0).normalized * 60f, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.3f);
                    rigid.velocity = Vector3.zero;
                    _curState = State.Idle;
                    break;
                case State.LongJump:
                    Vector3 jumpDiretion = new Vector3(playerPos.x - transform.position.x, 0, 0).normalized;
                    rigid.AddForce(new Vector3(jumpDiretion.x, 1, 0) * 40f, ForceMode2D.Impulse);
                    _curState = State.Idle;
                    break;
                case State.Cry:
                    // if (Vector3.Distance(playerPos, transform.position) <= 10)
                    // {
                    //     PlayerMoveinFixedUpdate.moveFlag = false;
                    //     PlayerMoveinFixedUpdate.lastTime = Time.time;
                    //     PlayerMoveinFixedUpdate.lastPos = playerPos;
                    // }
                    // yield return new WaitUntil(() => PlayerMoveinFixedUpdate.moveFlag);
                    // _curState = State.Idle;
                    break;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
}
