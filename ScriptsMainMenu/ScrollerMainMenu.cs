﻿using UnityEngine;
using System.Collections;

public class ScrollerMainMenu : MonoBehaviour {

    public float speed;

	// Update is called once per frame
	void Update () 
	{
        Vector2 offset = new Vector2(Time.time * speed, Time.time * speed);
			GetComponent<Renderer> ().material.mainTextureOffset = offset;
	}
		
}
