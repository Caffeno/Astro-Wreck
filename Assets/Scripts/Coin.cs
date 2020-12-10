using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    static int coinsCollected = 0;

    [SerializeField] private LayerMask playerLayer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hello World");

        if (collision.otherCollider.gameObject.layer == playerLayer.value)
        {
            coinsCollected += 1;
            Debug.Log(coinsCollected);
            GameObject.Destroy(gameObject);
        }
    }
}
