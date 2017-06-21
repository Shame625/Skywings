using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    Transform target;
    public bool caught = false;
    float speed = 15;

    void Update()
    {
        if (caught)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
        }
    }

    public void Caught(Transform playerObj)
    {
        target = playerObj;
        caught = true;
    }

}
