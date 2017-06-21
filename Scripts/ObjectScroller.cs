using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectScroller : MonoBehaviour {

	GameManager GM;
    public List<GameObject> childrenObstacles = new List<GameObject>();
    List<GameObject> coins = new List<GameObject>();


	void Awake()
	{
		GM = GameObject.FindGameObjectWithTag ("GameManager").GetComponent < GameManager>();
		Initialization();
		FindAllObstacleChildren();

	}


	void Update () {
		if (transform.position.x <= -20) 
		{
            ResetObject();
		}

            transform.position = Vector3.MoveTowards(transform.position, new Vector2(-20, transform.position.y), GM.platformSpeed * Time.fixedDeltaTime);
	}


    void Initialization()
    {
        int n = 0;
        for (int i = 0; i <= 6; i++)
        {
            GameObject go;
            go = (GameObject)Instantiate(GM.obstacle, new Vector2(transform.GetChild(0).transform.position.x, transform.GetChild(0).transform.position.y-(1.5f) + (1.5f * i)), Quaternion.identity);
            go.transform.SetParent(transform.GetChild(0).transform);
			//set texture
			if (GM.blockSprites.Length>0) 
			{
				go.GetComponent<SpriteRenderer> ().sprite = GM.blockSprites[n];
                if (i == 0)
                {
                    go.GetComponent<Renderer>().sortingOrder = 0;
                }
                if (GM.blockSprites.Length > 1)
                {
                    n++;
                    if (n >= GM.blockSprites.Length)
                        n = 0;
                }
			}
        }
        GameObject pillar;
        pillar = (GameObject)Instantiate(GM.pillar, new Vector2(transform.GetChild(0).transform.position.x, (transform.GetChild(0).transform.position.y + 5)), Quaternion.identity);
        pillar.transform.SetParent(transform.GetChild(0).transform);
    }

    public void ResetObject()
    {
        GenerateCoins();
        transform.position = GM.SetObject(gameObject);
        GenerateNewStructure(GM.GenerateCase());
        
    }



    public void GenerateNewStructure(int[] newStructure)
    {

        for (int i = 0; i < childrenObstacles.Count; i++)
        {
            if (newStructure[i] == 1)
            {
                childrenObstacles[i].SetActive(true);
            }
            else
                childrenObstacles[i].SetActive(false);
        }
    }

    void GenerateCoins()
    {
        foreach (GameObject coin in coins)
        {
            
            if (coin != null)
            {
                coin.GetComponent<Coin>().transform.SetParent(null);
                if (coin.GetComponent<Coin>().caught == false)
                {
                    Destroy(coin);
                }
            }
        }
        coins.Clear();
        int x = Random.Range(0,1000);


        if (x > 990)
        {
            foreach (Vector3 pos in GM.coinSpots)
            {
                GameObject go = (GameObject)Instantiate(GM.coin, transform.GetChild(1).transform.position + pos, Quaternion.identity);
                go.transform.SetParent(transform.GetChild(1).transform);
                coins.Add(go);
            }
        }

        else if (x < 250)
        {
            int n = Random.Range(0, 11);
            List<int> randomIndexes = new List<int>();
            switch(n)
            {
                case 0:
                    randomIndexes.Add(0);
                    randomIndexes.Add(1);
                    randomIndexes.Add(2);
                    break;
                case 1:
                    randomIndexes.Add(3);
                    randomIndexes.Add(4);
                    randomIndexes.Add(5);
                    break;
                case 2:
                    randomIndexes.Add(6);
                    randomIndexes.Add(7);
                    randomIndexes.Add(8);
                    break;
                case 3:
                    randomIndexes.Add(0);
                    randomIndexes.Add(3);
                    randomIndexes.Add(6);
                    break;
                case 4:
                    randomIndexes.Add(1);
                    randomIndexes.Add(4);
                    randomIndexes.Add(7);
                    break;
                case 5:
                    randomIndexes.Add(2);
                    randomIndexes.Add(5);
                    randomIndexes.Add(8);
                    break;
                case 6:
                    randomIndexes.Add(0);
                    randomIndexes.Add(2);
                    randomIndexes.Add(4);
                    randomIndexes.Add(6);
                    randomIndexes.Add(8);
                    break;
                case 7:
                    randomIndexes.Add(1);
                    randomIndexes.Add(3);
                    randomIndexes.Add(4);
                    randomIndexes.Add(5);
                    randomIndexes.Add(7);
                    break;
                case 8:
                    randomIndexes.Add(0);
                    randomIndexes.Add(1);
                    randomIndexes.Add(2);
                    randomIndexes.Add(3);
                    randomIndexes.Add(5);
                    randomIndexes.Add(6);
                    randomIndexes.Add(7);
                    randomIndexes.Add(8);
                    break;
                case 9:
                    randomIndexes.Add(0);
                    randomIndexes.Add(1);
                    randomIndexes.Add(2);
                    randomIndexes.Add(6);
                    randomIndexes.Add(7);
                    randomIndexes.Add(8);
                    break;
                case 10:
                    randomIndexes.Add(0);
                    randomIndexes.Add(3);
                    randomIndexes.Add(6);
                    randomIndexes.Add(2);
                    randomIndexes.Add(5);
                    randomIndexes.Add(8);
                    break;
			case 11:
				randomIndexes.Add(0);
				randomIndexes.Add(2);
				randomIndexes.Add(6);
				randomIndexes.Add(8);
				break;
            }

            foreach (int randIndex in randomIndexes)
            {
                GameObject go = (GameObject)Instantiate(GM.coin, transform.GetChild(1).transform.position + (Vector3)GM.coinSpots[randIndex], Quaternion.identity);
                go.transform.SetParent(transform.GetChild(1).transform);
                coins.Add(go);
            }
        }
    }


    void FindAllObstacleChildren()
    {
        int childCount = transform.GetChild(0).transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            childrenObstacles.Add(transform.GetChild(0).GetChild(i).gameObject);
        }
    }

}
