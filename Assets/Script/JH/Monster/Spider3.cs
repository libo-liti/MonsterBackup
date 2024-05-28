using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider3 : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform platformPos;
    bool isUp = false;
    void Start()
    {
        //transform.position = new Vector3(transform.position.x, platformPos.position.y + platformPos.localScale.y / 2.0f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp)
        {
            if (transform.position.y < platformPos.position.y + platformPos.localScale.y / 2.0f)
                transform.position += Vector3.up * 4 * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.W))
            isUp = true;
    }
}
