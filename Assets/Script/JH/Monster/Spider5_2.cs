using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider5_2 : MonoBehaviour
{
    public float angle = 0;

    private float lerpTime = 0;
    private float speed = 2f;
    public bool flag = false;

    private void Update()
    {
        if (!flag)
        {
            lerpTime += Time.deltaTime * speed;
            transform.rotation = CalculateMovementOfPendulum();
        }
        if (flag)
            Destroy(gameObject);
    }
    Quaternion CalculateMovementOfPendulum()
    {
        return Quaternion.Lerp(Quaternion.Euler(Vector3.forward * angle),
            Quaternion.Euler(Vector3.back * angle), GetLerpTParam());
    }

    float GetLerpTParam()
    {
        return (Mathf.Sin(lerpTime) + 1) * 0.5f;
    }
}
