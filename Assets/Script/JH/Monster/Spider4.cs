using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider4 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetComponent<Rigidbody2D>().gravityScale = 4;
        }
    }
}
