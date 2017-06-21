using UnityEngine;
using System.Collections;

public class PlayerBot : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            FindFreeSpot(other.transform.root.transform.root.gameObject);
        }
    }

    void FindFreeSpot(GameObject obstacle)
    {
        foreach (GameObject ob in obstacle.GetComponent<ObjectScroller>().childrenObstacles)
        {
            if (ob.activeSelf == false)
            {
                transform.root.GetComponent<Player>().Cheat(ob.transform.position);
                break;
            }
        }
    }

}
