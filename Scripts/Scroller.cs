using UnityEngine;
using System.Collections;

public class Scroller : MonoBehaviour {

	GameManager GM;

	void Awake()
	{
		GM = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (GM.moveBackground) 
		{
			Vector2	offset = new Vector2 (Time.time * GM.backgroundSpeed, 0);
			GetComponent<Renderer> ().material.mainTextureOffset = offset;
		}
	}
		
}
