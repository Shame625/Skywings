using UnityEngine;
using System.Collections;

public class Button_Move : MonoBehaviour {

	public AudioClip myClip;

	public void Clicked()
	{
		GetComponentInParent<SpawnMovable> ().PlayAudio (myClip);
	}
}
