using System.Collections;
using Cinemachine;
using UnityEngine;

public class Boss_Test : MonoBehaviour
{
    private enum State
    {
        Waiting,
        FirstAction,
        Idle,
        Earthquake,
        Cry,
        Tornado,
        Dash,
        Path,
        Throw
    }
    State _curState;
    Transform playerPos;
    CinemachineVirtualCamera cam;
    CinemachineBasicMultiChannelPerlin noise;
    GameObject WideCam;
    public GameObject path;
    public GameObject stone;
    public GameObject shadow;
    public GameObject hitRange;
    Rigidbody2D rigid;
    public GameObject[] walls;
    float fieldOfView;
    float initialRadius = 0f; // 초기 원의 반지름
    float maxRadius = 10f; // 최대 원의 반지름
    float expandSpeed = 2.5f; // 원이 커지는 속도
    float currentRadius; // 현재 원의 반지름
    int segments = 50; // 원을 그릴 때 사용하는 세그먼트의 수
    private LineRenderer lineRenderer;
    float curAttackNum = 0;
    float nextAttackNum;
    bool parrying = false;
    Vector3 lastPlayerPos;
    private void Awake()
    {
        _curState = State.Idle;
        rigid = GetComponent<Rigidbody2D>();
        fieldOfView = 15;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Boss"));
    }
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        currentRadius = initialRadius;
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;

        WideCam = GameObject.Find("Cam");
        cam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        StartCoroutine(Phase2());
    }
    private void Update()
    {
        if (_curState == State.Earthquake)
            playerPos = GameObject.Find("Player").transform;
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (rigid.gravityScale == 30f && Vector3.Distance(playerPos.position, transform.position) <= 7f)
            {
                rigid.velocity = Vector3.zero;
                rigid.gravityScale = 1;
                if (playerPos.localScale.x < 0)
                    rigid.AddForce(new Vector2(-4f, 5f), ForceMode2D.Impulse);
                else
                    rigid.AddForce(new Vector2(4f, 5f), ForceMode2D.Impulse);
                parrying = true;
                Invoke("Parrying", 5f);
                Debug.Log("패링");
            }
            Debug.Log(rigid.gravityScale);
            Debug.Log(Vector3.Distance(playerPos.position, transform.position));
        }
    }
    void Parrying()
    {
        parrying = false;
    }
    void CreatePoints(float radius)
    {
        float angle = 2 * Mathf.PI / segments;

        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(i * angle) * radius;
            float y = Mathf.Cos(i * angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 7f);
    }
    IEnumerator Phase1()
    {
        while (true)
        {
            playerPos = GameObject.Find("Player").transform;
            transform.localScale = new Vector3(Character_Direction(playerPos, transform), 1, 1);

            switch (_curState)
            {
                case State.Waiting:
                    if (Vector3.Distance(playerPos.position, transform.position) < fieldOfView)
                    {
                        _curState = State.FirstAction;
                        WideCam = GameObject.Find("Cam").transform.GetChild(0).gameObject;
                    }
                    break;
                case State.FirstAction:
                    WideCam.SetActive(true);
                    yield return new WaitForSeconds(3f);
                    walls[0].SetActive(true);
                    walls[1].SetActive(true);
                    yield return new WaitForSeconds(3f);
                    WideCam.SetActive(false);
                    walls[0].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    walls[1].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    _curState = State.Idle;
                    break;
                case State.Idle:
                    // if (Vector3.Distance(transform.position, playerPos.position) < 15)
                    // {
                    //     nextAttackNum = Random.Range(0, 4);
                    //     if (nextAttackNum == curAttackNum)
                    //         nextAttackNum = Random.Range(0, 4);
                    //     curAttackNum = nextAttackNum;
                    //     switch (curAttackNum)
                    //     {
                    //         case 0:
                    //             _curState = State.Earthquake;
                    //             break;
                    //         case 1:
                    //             _curState = State.Cry;
                    //             break;
                    //         case 2:
                    //             _curState = State.Tornado;
                    //             break;
                    //         case 3:
                    //             _curState = State.Dash;
                    //             break;
                    //     }
                    // }
                    // else
                    // {
                    //     nextAttackNum = Random.Range(0, 2);
                    //     if (nextAttackNum == curAttackNum)
                    //         nextAttackNum = Random.Range(0, 2);
                    //     curAttackNum = nextAttackNum;
                    //     switch (nextAttackNum)
                    //     {
                    //         case 0:
                    //             _curState = State.Tornado;
                    //             break;
                    //         case 1:
                    //             _curState = State.Dash;
                    //             break;
                    //     }
                    // }
                    if (Input.GetKeyDown(KeyCode.E))
                        _curState = State.Earthquake;
                    if (Input.GetKeyDown(KeyCode.D))
                        _curState = State.Dash;
                    if (Input.GetKeyDown(KeyCode.C))
                        _curState = State.Cry;
                    break;
                case State.Earthquake:
                    rigid.AddForce(Vector3.up * 400);
                    yield return new WaitUntil(() => rigid.velocity.y < 0);

                    rigid.gravityScale = 50f;

                    yield return new WaitUntil(() => rigid.velocity.y == 0);
                    noise.m_AmplitudeGain = 5;
                    noise.m_FrequencyGain = 1;

                    if (GameObject.Find("Player").GetComponent<Rigidbody2D>().velocity.y == 0)
                    {
                        PlayerMoveinFixedUpdate.moveFlag = false;
                        PlayerMoveinFixedUpdate.lastTime = Time.time;
                        PlayerMoveinFixedUpdate.lastPos = playerPos.position;
                    }
                    yield return new WaitForSeconds(1f);
                    noise.m_AmplitudeGain = 0;
                    noise.m_FrequencyGain = 0;
                    rigid.gravityScale = 1;

                    _curState = State.Idle;
                    yield return new WaitForSeconds(1f);
                    break;
                case State.Tornado:
                    yield return StartCoroutine(Blink_Color(gameObject, Color.green));
                    for (int i = 0; i < 100; i++)
                    {
                        float speed;
                        if (GameObject.Find("Player").GetComponent<Rigidbody2D>().velocity.y == 0)
                            speed = 0.5f;
                        else
                            speed = 0.1f;
                        transform.Rotate(0, 20, 0);
                        if (playerPos.position.x - transform.position.x >= 0)
                            transform.position += Vector3.right * speed;
                        else
                            transform.position += Vector3.left * speed;
                        yield return new WaitForSeconds(0.02f);
                    }
                    _curState = State.Idle;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    yield return new WaitForSeconds(1f);
                    break;
                case State.Dash:
                    yield return StartCoroutine(Blink(gameObject));
                    yield return new WaitForSeconds(1f);
                    rigid.AddForce(new Vector3(playerPos.position.x - transform.position.x, 0, 0).normalized * 60f, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.3f);
                    rigid.velocity = Vector3.zero;
                    _curState = State.Idle;
                    yield return new WaitForSeconds(1f);
                    break;
                case State.Cry:
                    yield return StartCoroutine(Blink_Color(gameObject, Color.red));
                    StartCoroutine(CryRange());
                    if (Vector3.Distance(playerPos.position, transform.position) <= 10)
                    {
                        Rigidbody2D rigid2d = playerPos.GetComponent<Rigidbody2D>();

                        PlayerMoveinFixedUpdate.flag = false;
                        Vector3 direction = (playerPos.position - transform.position).normalized;
                        rigid2d.AddForce(direction * 50f, ForceMode2D.Impulse);
                        yield return new WaitUntil(() => Vector3.Distance(playerPos.position, transform.position) > 10);
                        PlayerMoveinFixedUpdate.flag = true;
                    }
                    yield return new WaitUntil(() => PlayerMoveinFixedUpdate.moveFlag);
                    _curState = State.Idle;
                    yield return new WaitForSeconds(1f);
                    break;
            }
            yield return new WaitForSeconds(0.03f);
        }
    }
    IEnumerator Phase2()
    {
        while (true)
        {
            playerPos = GameObject.Find("Player").transform;
            transform.localScale = new Vector3(Character_Direction(playerPos, transform), 1, 1);
            switch (_curState)
            {
                case State.Idle:
                    if (Input.GetKeyDown(KeyCode.E))
                        _curState = State.Earthquake;
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        _curState = State.Dash;
                    }
                    // if (Input.GetKeyDown(KeyCode.C))
                    //     _curState = State.Cry;
                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        _curState = State.Path;
                    }
                    if (Input.GetKeyDown(KeyCode.T))
                    {
                        _curState = State.Throw;
                    }
                    break;
                case State.Earthquake:
                    transform.position = playerPos.position + Vector3.up * 40;
                    shadow.SetActive(true);
                    shadow.transform.position = new Vector3(playerPos.position.x,
                    shadow.transform.position.y, -1);
                    yield return new WaitForSeconds(1f);
                    rigid.gravityScale = 30f;
                    yield return new WaitUntil(() => rigid.velocity.y == 0 || rigid.gravityScale == 1f);
                    if (!parrying && Vector3.Distance(playerPos.position, transform.position) <= 5)
                    {
                        PlayerMoveinFixedUpdate.flag = false;
                        if (Character_Direction(playerPos, transform) == 1)
                            GameObject.Find("Player").GetComponent<Rigidbody2D>().AddForce(Vector3.right * 30, ForceMode2D.Impulse);
                        else if (Character_Direction(playerPos, transform) == -1)
                            GameObject.Find("Player").GetComponent<Rigidbody2D>().AddForce(Vector3.left * 30, ForceMode2D.Impulse);
                        else
                            GameObject.Find("Player").GetComponent<Rigidbody2D>().AddForce(Vector3.left * 1f, ForceMode2D.Impulse);
                        Invoke("playerFlag", 0.2f);
                    }
                    shadow.SetActive(false);
                    hitRange.SetActive(true);
                    hitRange.transform.position = new Vector3(shadow.transform.position.x,
                    hitRange.transform.position.y, -1);
                    Invoke("HitRange", 0.4f);
                    rigid.gravityScale = 1f;
                    _curState = State.Idle;
                    break;
                case State.Dash:
                    yield return StartCoroutine(Blink(gameObject));
                    yield return new WaitForSeconds(1f);
                    rigid.AddForce((playerPos.position - transform.position).normalized * 60f, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.3f);
                    rigid.velocity = Vector3.zero;
                    yield return new WaitForSeconds(0.1f);
                    rigid.AddForce((playerPos.position - transform.position).normalized * 60f, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.3f);
                    rigid.velocity = Vector3.zero;
                    _curState = State.Idle;
                    yield return new WaitForSeconds(1f);
                    break;
                case State.Path:
                    lastPlayerPos = playerPos.position;
                    for (int i = 0; i < 10; i++)
                    {
                        Instantiate(path, new Vector3(lastPlayerPos.x + 5 * i, 180, 0), Quaternion.identity);
                        Instantiate(path, new Vector3(lastPlayerPos.x + 5 * -i, 180, 0), Quaternion.identity);
                        yield return new WaitForSeconds(0.42f);
                        Debug.Log(playerPos.position.x);
                    }
                    _curState = State.Idle;
                    break;
                case State.Throw:
                    // for (int i = 0; i < 5; i++)
                    // {
                    //     GameObject obj = Instantiate(stone, transform.position, Quaternion.identity);
                    //     obj.GetComponent<Rigidbody2D>().AddForce(new Vector3(playerPos.position.x - transform.position.x, 0, 0).normalized * 30, ForceMode2D.Impulse);
                    //     yield return new WaitForSeconds(0.3f);
                    // }
                    for (int i = 0; i < 5; i++)
                    {
                        GameObject obj = Instantiate(stone, transform.position, Quaternion.identity);
                        obj.GetComponent<Rigidbody2D>().AddForce(new Vector3(playerPos.position.x - transform.position.x, 0, 0).normalized * 30, ForceMode2D.Impulse);
                        yield return new WaitForSeconds(0.5f);
                    }
                    _curState = State.Idle;
                    break;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    void HitRange()
    {
        hitRange.SetActive(false);
    }
    int Character_Direction(Transform target, Transform myself)
    {
        if (target.position.x - myself.position.x > 0)
            return 1;
        else if (target.position.x - myself.position.x < 0)
            return -1;
        return 0;
    }
    IEnumerator Blink(GameObject obj)
    {
        SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
        for (int i = 0; i < 2; i++)
        {
            sprite.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(0.2f);
            sprite.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.2f);
        }
    }
    IEnumerator Blink_Color(GameObject obj, Color color)
    {
        SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
        Color origin = sprite.color;
        for (int i = 0; i < 3; i++)
        {
            sprite.color = color;
            yield return new WaitForSeconds(0.2f);
            sprite.color = origin;
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {

    }
    void playerFlag()
    {
        PlayerMoveinFixedUpdate.flag = true;
    }
    IEnumerator CryRange()
    {
        while (currentRadius < maxRadius)
        {
            currentRadius += expandSpeed; // 원의 반지름을 점차 증가
            CreatePoints(currentRadius);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        currentRadius = 0;
        CreatePoints(currentRadius);
    }
}
