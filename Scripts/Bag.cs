using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bag : MonoBehaviour {
    public bool ranIntro;

    public Sprite PlayerSkin;
    
	public List<Sprite> playerSprites = new List<Sprite> ();
	//Map skin stuff
	public List<Sprite> mapTiles = new List<Sprite> ();
	public List<Sprite> blockTiles = new List<Sprite> ();

	public float speedModifier;
	public float magnetModifier;

	//bad way, hardcoded
    public bool space = false;
	public bool autoPilot = false;
	public bool reverseMode = false;
    public bool hyperMode = false;
    public bool hyperModeExtreme = false;
    public bool x2Speed = false;
    public bool x2Speed2 = false;

    public bool doubleCoin = false;
    public bool doubleCoin2 = false;

    public bool isMuted = false;



    void Awake()
    {
        isMuted = PlayerPrefs.GetInt("Muted")==1;
        DontDestroyOnLoad(transform.gameObject);
    }


    public void SetPlayerSkin(Sprite playerSkin)
    {
        PlayerSkin = playerSkin;
    }

	public void EffectCheck(string name, bool condition)
	{
        if (name == "Space")
        {
            space = condition;
        }

		if (name == "Auto Pilot") 
		{
			autoPilot = condition;
		}
		if (name == "Reverse Mode") 
		{
			reverseMode = condition;
		}

        if (name == "Double Speed")
        {
            x2Speed = condition;
        }
        if (name == "Double Speed 2")
        {
            x2Speed2 = condition;
        }
        if (name == "Hyper Mode")
        {
            hyperMode = condition;
        }
        if (name == "Hyper Mode Extreme")
        {
            hyperModeExtreme = condition;
        }

        if (name == "Double Coin")
        {
            doubleCoin = condition;
        }
        if (name == "Double Coin 2")
        {
            doubleCoin2 = condition;
        }
	}



    public void Volume_Setting()
    {
        isMuted = PlayerPrefs.GetInt("Muted") == 1;
        isMuted = !isMuted;

        //converts bool to int
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
    }
}
