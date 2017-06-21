using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenuManager : MonoBehaviour {

    public List<GameObject> UIElements = new List<GameObject>();

    AudioSource audioSource;

    public GameObject shopItem;

    public GameObject confirmWindow;

    public GameObject listContainer;
    public GameObject characterSkinList;
    public GameObject mapSkinList;
    public GameObject playerItemsList;
    public GameObject specialList;
    public GameObject[] panelButtons;

    public GameObject Main_Menu;
    public GameObject Info_Panel;
    public GameObject Shop_Panel;
    public GameObject Credits_Panel;
    public GameObject Buttons;
    public GameObject volumeButton;
    public GameObject screenFader;

    int score;
    int playerCoins;
    int totalScore;
    int totalCoinsEverHad;
    int totalDeaths;
    int maximumCoinsInOneGame;
    string ownedItems;

	GameObject currentUiElement;
	GameObject previousUiElement;
	public Sprite normalButtonSprite;
	public Sprite focusedButtonSprite;
	public Sprite alreadyOwnedButtonSprite;

    public Sprite[] volumeSprites;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        audioSource.Play();

        screenFader.GetComponent<SceneFader>().GoToNextScene("game");
    }

    public void QuitGame()
    {
        audioSource.Play();

        Application.Quit();
    }

    public void OpenInfoPanel(bool open)
    {
        audioSource.Play();

        if (open == true)
        {
            Info_Panel.SetActive(true);
            Buttons.SetActive(false);
            Info();
        }
        else
        {
            Info_Panel.SetActive(false);
            Buttons.SetActive(true);
        }
    }


    public void OpenShopPanel(bool open)
    {
        audioSource.Play();

        if (open == true)
        {
            Shop_Panel.SetActive(true);
            Main_Menu.SetActive(false);
            Shop();
        }
        else
        {
            Shop_Panel.SetActive(false);
            Main_Menu.SetActive(true);
        }
    }

    public void OpenCreditsPanel()
    {
        audioSource.Play();

            Credits_Panel.SetActive(!Credits_Panel.activeSelf);
            Main_Menu.SetActive(!Main_Menu.activeSelf);
    }

    void Shop()
    {
        Load();

        Shop_Panel.transform.GetChild(1).GetComponent<Text>().text = "Coins: " + playerCoins.ToString();

    }

	public void SetNewShopCoinsValue()
	{
		playerCoins = PlayerPrefs.GetInt("Coins");
		Shop_Panel.transform.GetChild(1).GetComponent<Text>().text = "Coins: " + playerCoins.ToString();
	}

    public void OpenConfirmWindow(bool condition)
    {
        confirmWindow.SetActive(condition);
        Shop();
    }

    public void Shop_Item_Window_Manager(int display)
    {
        ButtonFadingOut(display);
        audioSource.Play();
        if (display == 0)
        { 
            characterSkinList.SetActive(true);
            mapSkinList.SetActive(false);
            playerItemsList.SetActive(false);
            specialList.SetActive(false);
            listContainer.GetComponent<ScrollRect>().content = characterSkinList.GetComponent<RectTransform>();
        }
        else if (display == 1)
        {
            characterSkinList.SetActive(false);
            mapSkinList.SetActive(true);
            playerItemsList.SetActive(false);
            specialList.SetActive(false);
            listContainer.GetComponent<ScrollRect>().content = mapSkinList.GetComponent<RectTransform>();
        }
        else if (display == 2)
        {
            characterSkinList.SetActive(false);
            mapSkinList.SetActive(false);
            playerItemsList.SetActive(true);
            specialList.SetActive(false);
            listContainer.GetComponent<ScrollRect>().content = playerItemsList.GetComponent<RectTransform>();
        }
        else if (display == 3)
        {
            characterSkinList.SetActive(false);
            mapSkinList.SetActive(false);
            playerItemsList.SetActive(false);
            specialList.SetActive(true);
            listContainer.GetComponent<ScrollRect>().content = specialList.GetComponent<RectTransform>();
        }
    }

    void ButtonFadingOut(int i)
    {
        int buttonCount = panelButtons.Length - 1;
        int counter = 0;
        while (counter <= buttonCount)
        {
            if (i == counter)
            {
                panelButtons[counter].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                panelButtons[counter].GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
            }
            counter++;
        }
    }

    public void CreateShopItems(string name, string displayName, string ItemType, Sprite sprite, int cost, string note, bool owned)
    {
        GameObject newUIElement = (GameObject)Instantiate(shopItem, Vector3.zero, Quaternion.identity);

		newUIElement.GetComponent<SetShopObjProperties>().SetProperties(name, displayName, sprite, cost, note, owned, ItemType);
		if (owned) 
		{
			newUIElement.GetComponent<Image> ().sprite = alreadyOwnedButtonSprite;
		}
        switch (ItemType)
        { 
            case "player-skin":
                newUIElement.transform.SetParent(characterSkinList.transform);
                break;
            
            case "map-skin":
                newUIElement.transform.SetParent(mapSkinList.transform);
                break;

            case "player-item":
                newUIElement.transform.SetParent(playerItemsList.transform);
                break;

            case "special":
                newUIElement.transform.SetParent(specialList.transform);
                break;
        }
        UIElements.Add(newUIElement);
    }

    void Info()
    { 
        Load();
        string newText = "Highest Score: " + score.ToString() + "\nCoins: " + playerCoins.ToString() + "\n\nTotal Score: " + totalScore.ToString() 
            + "\nTotal Coins: " + totalCoinsEverHad.ToString() + "\n\nTotal Deaths: " + totalDeaths.ToString() + "\nMax coins in one game: " + maximumCoinsInOneGame.ToString();

        Info_Panel.transform.GetChild(1).GetComponent<Text>().text = newText;
    }

	public void SetAlreadyOwnedButton(GameObject button)
	{
		button.GetComponent<Image> ().sprite = alreadyOwnedButtonSprite;
		currentUiElement = null;
		previousUiElement = null;
	}

	public void SetFocusedButton(GameObject button)
	{
		
		if (currentUiElement != null) 
		{
			previousUiElement = currentUiElement;
			currentUiElement.GetComponent<Image> ().sprite = normalButtonSprite;
		}
		currentUiElement = button;

		currentUiElement.GetComponent<Image> ().sprite = focusedButtonSprite;
	}

	public void UnselectAll()
	{
		if (currentUiElement != null) 
		{
			currentUiElement.GetComponent<Image> ().sprite = normalButtonSprite;
		}
		if (previousUiElement != null) 
		{
			previousUiElement.GetComponent<Image> ().sprite = normalButtonSprite;
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

    public void SetVolumeButton(bool check)
    {
        if (check)
            volumeButton.GetComponent<Image>().sprite = volumeSprites[0];
        else
            volumeButton.GetComponent<Image>().sprite = volumeSprites[1];
    }
}
