using UnityEngine;
using System.Collections;

public class SpawnMovable : MonoBehaviour {
	public float startingPoint = 3.25f;
	public float offSet;
	public int amount;

	public AudioClip sound;
	AudioSource myAudio;
	public GameObject movableButton;

	void Awake()
	{
		myAudio = GetComponent<AudioSource> ();
		GenerateMovable (amount);
	}


	void GenerateMovable(int amount)
	{
		for (int i = 0; i <= amount; i++) {
			Vector2 position = new Vector2 (transform.position.x, startingPoint + i * offSet);
			GameObject go = (GameObject)Instantiate (movableButton, position, Quaternion.identity);
			go.transform.SetParent (transform);
			go.GetComponent<Button_Move> ().myClip = sound;
		}
	}

	public void PlayAudio(AudioClip clip)
	{
		myAudio.clip = clip;

		myAudio.Play();

	}
}
