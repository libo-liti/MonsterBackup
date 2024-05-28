using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderPlatform : MonoBehaviour
{
    public Spider[] spider;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            spider[0].isActive = true;
            spider[1].isActive = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            spider[0].isActive = true;
            spider[1].isActive = true;
            spider[0].timeOffPlatform = 0;
            spider[1].timeOffPlatform = 0;
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            spider[0].isActive = false;
            spider[1].isActive = false;
        }
    }
}
