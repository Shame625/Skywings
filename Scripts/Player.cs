using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public List<Sprite> playerSprites = new List<Sprite> ();

	public float speed;
	float SpeedMod;
	Vector2 moveToPosition;
	Vector2 nextPosition;
	bool canMove = true;
	public GameManager GM;

	public bool alive = true;

	AudioSource myAudio;
	public AudioClip death;
	public AudioClip deathFast;
    public AudioClip coinPickup;

    public GameObject magnet;

    int start_fps = 15;
    int fps;
	int index = 0;
	SpriteRenderer myRender;
	void Awake()
	{
        fps = start_fps;
		GM = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		myAudio = GetComponent<AudioSource> ();
		myRender = transform.GetChild (1).GetComponent<SpriteRenderer> ();
		//Check if skin is animated

	}
	void Start()
	{
        fps = start_fps + (int)GM.platformSpeed;
		moveToPosition = transform.position;
		nextPosition = moveToPosition;
		if (playerSprites.Count == 1) {
			myRender.sprite = playerSprites [index];
		}
	}

	void Update()
	{
        
		//lazy pc controlls
		if (canMove && new Vector2 (transform.position.x, transform.position.y) == nextPosition) {
			if (Input.GetKeyDown (KeyCode.Alpha1)) {
				moveToPosition = new Vector2 (transform.position.x, 9.25f);
				nextPosition = moveToPosition;
			}

			if (Input.GetKeyDown (KeyCode.Alpha2)) {
				moveToPosition = new Vector2 (transform.position.x, 7.75f);
				nextPosition = moveToPosition;
			}

			if (Input.GetKeyDown (KeyCode.Alpha3)) {
				moveToPosition = new Vector2 (transform.position.x, 6.25f);
				nextPosition = moveToPosition;
			}

			if (Input.GetKeyDown (KeyCode.Alpha4)) {
				moveToPosition = new Vector2 (transform.position.x, 4.75f);
				nextPosition = moveToPosition;
			}

			if (Input.GetKeyDown (KeyCode.Alpha5)) {
				moveToPosition = new Vector2 (transform.position.x, 3.25f);
				nextPosition = moveToPosition;
			}
		}
		//lel


        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
			Movement (false);
		}

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
			Movement (true);
		}

		if (Input.GetMouseButtonDown (0)) {
			ClickMovement ();
		}

        if (Input.GetKeyDown(KeyCode.B))
        {
			Bot(!transform.GetChild(4).gameObject.activeSelf);
        }
			
		if (alive) 
		{
			//sprite draw
			if (playerSprites.Count >1 && alive == true)
			{
                myRender.sprite = playerSprites[index];
				index = (int)((Time.time * fps) % playerSprites.Count);
				
                if (index == playerSprites.Count-1)
                {
                    fps = start_fps + (int)GM.platformSpeed;
                }
			}

			if (SpeedMod == 0) {
				transform.position = Vector2.MoveTowards (transform.position, moveToPosition, speed * Time.deltaTime);
			} 
			else if (SpeedMod == 6) 
			{
				transform.position = moveToPosition;
			}
			else
				transform.position = Vector2.MoveTowards (transform.position, moveToPosition, (SpeedMod*speed) * Time.deltaTime);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Ground") 
		{
			canMove = false;
		}
        if (other.tag == "Coin")
        {
            CoinGet(other.transform.gameObject);
        }
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Ground" && alive == true) 
		{
			canMove = true;
			GM.IncreaseScore ();
		}
	}
	void Movement(bool direction)
	{
		if (canMove && new Vector2(transform.position.x, transform.position.y) == nextPosition) {
			if (!direction && transform.position.y > 3.25f) {
				moveToPosition = new Vector2 (transform.position.x, transform.position.y - 1.5f);
			}

			if (direction && transform.position.y < 8f) {
				moveToPosition = new Vector2 (transform.position.x, transform.position.y + 1.5f);
			}
			nextPosition = moveToPosition;
		}
	}

	void ClickMovement()
	{
		if (canMove && alive) {
			RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
			if (hit.collider != null) {
				if (hit.collider.tag == "Move_button") {
					hit.collider.gameObject.GetComponent<Button_Move> ().Clicked ();
                    moveToPosition = new Vector2(transform.position.x, hit.collider.transform.position.y);
					nextPosition = moveToPosition;
				}
			}
		}
	}

	public void Death()
	{
		if (alive) 
		{
            
            transform.GetChild(1).gameObject.GetComponent<Animation>().Stop();
			if (GM.platformSpeed >= 15) {
				PlayAudio(deathFast);
			} else {
                PlayAudio(death);
			}
            GM.Death();
			alive = false;
			Invoke ("DramaticEffect", 0.1f);
			alive = false;
			
		}
	}

	void DramaticEffect()
	{
		GetComponent<Rigidbody2D> ().isKinematic = false;
	}

    public void CoinGet(GameObject coin)
    {
        PlayAudio(coinPickup);
        GM.IncreaseCoins();
        Destroy(coin);
    }

	public void Bot(bool condition)
    {
		transform.GetChild(4).gameObject.SetActive(condition);
    }

    public void Cheat(Vector2 moveToPos)
    {
        moveToPosition = new Vector2(transform.position.x, moveToPos.y);
        nextPosition = moveToPosition;
    }


    void PlayAudio(AudioClip clip)
    {
        myAudio.pitch = Random.Range(0.95f, 1.05f);
        myAudio.clip = clip;
        myAudio.Play();
    }

	public void SetProperties(float speedMod, float magnetMod)
	{
		SpeedMod = speedMod;

		if (magnetMod > 0) 
		{
			transform.GetChild (3).gameObject.SetActive (true);
			transform.GetChild (3).GetComponent<CircleCollider2D> ().radius = magnetMod;
		}
	}
}
