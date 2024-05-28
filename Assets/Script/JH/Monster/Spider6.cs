using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider6 : MonoBehaviour
{
    public GameObject attack;
    float shootTime = 3f;
    float lastShootTime = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = GameObject.Find("Player").transform.position;
        if (Vector3.Distance(playerPos, transform.position) < 20)
        {
            if (playerPos.x < transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);

            if (Time.time - lastShootTime > shootTime)
            {
                lastShootTime = Time.time;
                GameObject obj;
                obj = Instantiate(attack, transform.position, Quaternion.identity);
                obj.GetComponent<Rigidbody2D>().AddForce((playerPos - transform.position).normalized * 1400);
            }
        }
    }
}
