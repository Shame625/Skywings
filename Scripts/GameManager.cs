using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;
using System.Collections.Generic;



public class GameManager : MonoBehaviour
{
    public GameObject screenFader;
    AudioSource audioSource;
    public AudioSource[] audioSources;
    public GameObject backgroundQuad;
    public Texture2D[] background;
    public Texture2D[] specialBackgrounds;
    int playerCoins;
    int playerCurrentCoins;
    int score;
    int currentScore = 0;

    public Sprite[] mapSprites;
    public Sprite[] blockSprites;
    public GameObject mapObject;

    public GameObject pillar;
    public GameObject obstacle;

    public int mapObjects_count;
    public float mapStartingLocation;

    private List<int[]> obstacleCases = new List<int[]>();
    public List<Vector2> coinSpots = new List<Vector2>();


    GameObject LastObject;

    float startingPlatformSpeed;
    public float platformSpeed;
    public float backgroundSpeed;
    public bool moveBackground = true;

    public GameObject playerObject;
    public Vector2 playerStartingPosition;
    public GameObject[] ground;
    public GameObject coin;
    public bool GameInProgress = true;

    UIManager UI;
    Bag bag;


    //Cool player info
    int totalScore;
    int totalCoinsEverHad;
    int totalDeaths;
    int maximumCoinsInOneGame;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        Load();
        if (GameObject.FindGameObjectWithTag("Bag") != null)
        {
            bag = GameObject.FindGameObjectWithTag("Bag").GetComponent<Bag>();
        }
        UI = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
        startingPlatformSpeed = platformSpeed;
        GenerateCoinSpots();
        LoadCases();

        blockSprites = bag.blockTiles.ToArray();
        GenerateMap(mapObjects_count);
        Load_Volumes();
    }

    void Start()
    {
    //Handling some specials in here
        if (bag.hyperModeExtreme == true)
        {
            startingPlatformSpeed = 50;
            platformSpeed = 50;
        }
        else if (bag.hyperMode == true)
        {
            startingPlatformSpeed = 30;
            platformSpeed = 30; 
        }
        else
        {
            startingPlatformSpeed = 7;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }


    void GenerateMap(int mapSize)
    {
        GameObject mapObj;
        int n = 0;
        for (int i = 0; i <= mapSize; i++)
        {

            mapObj = (GameObject)Instantiate(mapObject, new Vector2(mapStartingLocation + (mapObject.transform.GetChild(2).transform.localScale.x * 10) * i, 0), Quaternion.identity);
            if (bag.mapTiles.Count > 0)
            {

                mapObj.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = bag.mapTiles[n];
                n++;
                if (n >= bag.mapTiles.Count)
                    n = 0;
            }
            if (i == mapObjects_count)
            {
                LastObject = mapObj;
            }
            if (i >= mapObjects_count - 1)
            {
                mapObj.GetComponent<ObjectScroller>().GenerateNewStructure(GenerateCase());
            }
        }

        //Set Background
        SetBackGround();
        //Instantiates player
        SpawnPlayer();
    }

    void SetBackGround()
    {
        if (bag.space)
        {
            backgroundQuad.GetComponent<MeshRenderer>().material.mainTexture = specialBackgrounds[0];
        }
        else
        {
            backgroundQuad.GetComponent<MeshRenderer>().material.mainTexture = background[Random.Range(0, background.Length)];
        }
    }
    void SpawnPlayer()
    {
        GameObject playerObj = (GameObject)Instantiate(playerObject, playerStartingPosition, Quaternion.identity);
        if (bag.gameObject != null)
        {
            if (playerObj != null && bag.playerSprites.Count > 0)
            {
                //playerObj.transform.GetChild (1).GetComponent<SpriteRenderer> ().sprite = bag.PlayerSkin;
                playerObj.GetComponent<Player>().playerSprites = bag.playerSprites;
            }
            playerObj.GetComponent<Player>().SetProperties(bag.speedModifier, bag.magnetModifier);
        }

        //speical effects
        if (bag.autoPilot)
        {
            playerObj.GetComponent<Player>().Bot(true);
            UI.SetAutoPilotOn();
        }
        if (bag.reverseMode)
        {
            playerObj.transform.localScale = new Vector3(1, -1, 1);
        }


        //Mute player
        playerObj.GetComponent<AudioSource>().mute = bag.isMuted;
    }
    //SetsObject to the end
    public Vector2 SetObject(GameObject go)
    {
        Vector2 newPos = new Vector2(LastObject.transform.position.x + go.transform.GetChild(2).transform.localScale.x * 10, go.transform.position.y);
        LastObject = go;
        return newPos;
    }

    //Lazy way to Restart, change this!
    public void Restart()
    {
        audioSource.Play();
        //Set variables to 0
        playerCurrentCoins = 0;
        currentScore = 0;
        //Destroys all obstacles
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Floor"))
        {
            Destroy(obj);
        }
        //Destroys player object
        Destroy(GameObject.FindGameObjectWithTag("Player"));


        platformSpeed = startingPlatformSpeed;
        moveBackground = true;
        UI.SetScore(0);
        UI.SetCoins(0);
        UI.CloseDeadPanel();
        //Generates obstacles
        GenerateMap(mapObjects_count);
        GameInProgress = true;
    }

    //Death sequence
    public void Death()
    {
        totalDeaths++;
        //Saves players current score and money  
        Save();
        GameInProgress = false;
        platformSpeed = 0;
        moveBackground = false;
        UI.DeadPanel(true, currentScore, score, playerCurrentCoins, playerCoins);
    }

    //Current crappy score handle for UI
    public void IncreaseScore()
    {
        if (bag.autoPilot == false)
        {
            currentScore++;
            if (bag.x2Speed2 && bag.x2Speed)
            {
                currentScore += 3;
            }
            else
            {
                if (bag.x2Speed)
                {
                    currentScore += 1;
                }
                if (bag.x2Speed2)
                {
                    currentScore += 1;
                }
            }
            UI.SetScore(currentScore);
        }
        IncreaseDifficulty();
    }

    public void IncreaseCoins()
    {
        if (bag.doubleCoin == false && bag.doubleCoin2 == false)
        {
            playerCurrentCoins++;
            
        }
        else
        {
            if (bag.doubleCoin)
            {
                playerCurrentCoins += 2;
            }
            if (bag.doubleCoin2)
            {
                playerCurrentCoins += 2;
            }
        }
        UI.SetCoins(playerCurrentCoins);
    }

    //Change
    public void IncreaseDifficulty()
    {
        if (platformSpeed < 25 && GameInProgress == true)
        {
            platformSpeed += 0.1f;
            //Increase it once more
            if (bag.x2Speed2 && bag.x2Speed)
            {
                platformSpeed += 0.3f;
            }
            else
            {
                if (bag.x2Speed)
                {
                    platformSpeed += 0.1f; 
                }
                if (bag.x2Speed2)
                {
                    platformSpeed += 0.1f;
                }
            }
        }
    }

    //Cases for incoming obstacles, can move to xml file eventaulyl if I want to
    void LoadCases()
    {
        obstacleCases.Add(new int[] { 1, 0, 1, 1, 1, 1, 1, 1 });
        obstacleCases.Add(new int[] { 1, 1, 0, 1, 1, 1, 1, 1 });
        obstacleCases.Add(new int[] { 1, 1, 1, 0, 1, 1, 1, 1 });
        obstacleCases.Add(new int[] { 1, 1, 1, 1, 0, 1, 1, 1 });
        obstacleCases.Add(new int[] { 1, 1, 1, 1, 1, 0, 1, 1 });
        obstacleCases.Add(new int[] { 1, 0, 1, 0, 1, 0, 1, 1 });
        obstacleCases.Add(new int[] { 1, 0, 1, 0, 1, 1, 1, 1 });
        obstacleCases.Add(new int[] { 1, 1, 0, 1, 0, 1, 1, 1 });
    }

    //Generates new case
    public int[] GenerateCase()
    {
        int[] newCase;
        newCase = obstacleCases[Random.Range(0, obstacleCases.Count)];
        return newCase;
    }

    void GenerateCoinSpots()
    {
        float y = 1.5f;
        while (y <= 4.5f)
        {
            float x = 1.75f;
            while (x <= 6.25f)
            {
                coinSpots.Add(new Vector2(x, y));
                x += 1.75f;
            }
            y += 1.5f;
        }

    }


    void Save()
    {
        playerCoins += playerCurrentCoins;
        PlayerPrefs.SetInt("Coins", playerCoins);
        if (currentScore > score)
        {
            score = currentScore;
            PlayerPrefs.SetInt("Score", score);
        }


        //Cool info stuff
        PlayerPrefs.SetInt("TotalDeaths", totalDeaths);

        totalScore += currentScore;
        PlayerPrefs.SetInt("TotalScore", totalScore);

        totalCoinsEverHad += playerCurrentCoins;
        PlayerPrefs.SetInt("TotalCoinsEverHad", totalCoinsEverHad);

        if (playerCurrentCoins > PlayerPrefs.GetInt("MaximumCoinsInOneGame"))
        {
            maximumCoinsInOneGame = playerCurrentCoins;
            PlayerPrefs.SetInt("MaximumCoinsInOneGame", maximumCoinsInOneGame);
        }


    }

    void Load()
    {
        score = PlayerPrefs.GetInt("Score");
        playerCoins = PlayerPrefs.GetInt("Coins");

        totalScore = PlayerPrefs.GetInt("TotalScore");
        totalCoinsEverHad = PlayerPrefs.GetInt("TotalCoinsEverHad");
        totalDeaths = PlayerPrefs.GetInt("TotalDeaths");
        maximumCoinsInOneGame = PlayerPrefs.GetInt("MaximumCoinsInOneGame");
    }

    public void MainMenu()
    {
        audioSource.Play();
        Destroy(bag.gameObject);
        screenFader.GetComponent<SceneFader>().GoToNextScene("mainMenu");
    }


    void Load_Volumes()
    {
        foreach (AudioSource source in audioSources)
        {
            source.mute = bag.isMuted;
        }
    }
}
