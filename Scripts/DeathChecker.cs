using UnityEngine;
using System.Collections;

public class DeathChecker : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Ground") 
		{
			GetComponentInParent<Player> ().Death ();
		}
	}
}
