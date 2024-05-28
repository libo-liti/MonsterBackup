using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spider5 : MonoBehaviour
{
    public float rotationSpeed = 100.0f; // 회전 속도
    private bool isFalling = false; // 떨어지고 있는지 상태 확인

    void Update()
    {
        // isFalling이 true인 동안에만 회전
        if (isFalling)
        {
            transform.Rotate(0, 0, 40);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            transform.parent.parent.GetComponent<Spider5_2>().flag = true;
            isFalling = true;
            transform.SetParent(null);
            GetComponent<Rigidbody2D>().gravityScale = 2f;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        isFalling = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = new Vector3(transform.position.x,
        other.transform.position.y + other.transform.localScale.y / 2.0f, transform.position.z);
    }
}
