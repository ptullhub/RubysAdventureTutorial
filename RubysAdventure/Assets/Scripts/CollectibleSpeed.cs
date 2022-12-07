using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpeed : MonoBehaviour
{
    public AudioClip collectedClip;
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeSpeed(1);
            controller.PlaySound(collectedClip);
            Destroy(gameObject);
        }

    }
}

