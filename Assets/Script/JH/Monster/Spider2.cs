using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spider2 : MonoBehaviour
{
    // Update is called once per frame
    public Transform platformPos;
    bool isDown = false;
    private void Awake()
    {

    }
    void Update()
    {
        if (isDown)
            transform.position += Vector3.down * Time.deltaTime * 5;
        if (Input.GetKeyDown(KeyCode.D))
            isDown = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        isDown = false;
        Destroy(transform.GetChild(0).gameObject);
        transform.Rotate(0, 0, -90);
        transform.position = new Vector3(transform.position.x, platformPos.position.y, transform.position.z);
    }
}
