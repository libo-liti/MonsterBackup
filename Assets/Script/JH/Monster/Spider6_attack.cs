using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider6_attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}
