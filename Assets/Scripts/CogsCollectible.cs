using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogsCollectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeCogs(4);
            Destroy(gameObject);
        }
    }
}
