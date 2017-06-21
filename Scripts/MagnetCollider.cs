using UnityEngine;
using System.Collections;

public class MagnetCollider : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Collectible")
        {
            if (other.gameObject != null)
            {
				other.gameObject.GetComponent<Coin>().Caught(GetComponentInParent<Player>().transform);
            }
        }
    }

}
