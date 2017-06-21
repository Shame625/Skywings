using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Collectible")
        {
            GetComponentInParent<Player>().CoinGet(other.transform.gameObject);
        }
    }
}
