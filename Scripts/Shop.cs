using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Shop : MonoBehaviour{

    mainMenuManager MM;
    Bag bag;

	AudioSource myAudioSource;
	public AudioClip click;
	public AudioClip saleDone;
	public AudioClip notEnoughMoney;

    int playerCoins;
    public const string path = "items";

    public List<StoreItem> StoreItems = new List<StoreItem>();

    string ownedItems;
	public List<string> ownedItemsList = new List<string> ();

    int current_buying_id;

    string previousObjectType; 
    GameObject currentUiElement;
    GameObject previousUiElement;
    string currentPlayerSkinInUse;
    string currentMapSkinInUse;


	public List<string> activeSpecialsList = new List<string> ();
	public string activeSpecials;

    void Awake()
    {
		Load();
		myAudioSource = GetComponent<AudioSource> ();
        MM = GameObject.FindGameObjectWithTag("UI").GetComponent<mainMenuManager>();
        bag = GameObject.FindGameObjectWithTag("Bag").GetComponent<Bag>();

		MM.SetNewShopCoinsValue ();
        LoadPlayerAndMapSkin();
    }

    public class StoreItem
    {
        public string Name;
		public string DisplayName;
        public Sprite ItemSprite;
        public string ItemType;
        public int Cost;
        public string Note;
		public string Effect;

		public StoreItem(string name, string displayName, Sprite itemSprite, string itemType, int cost, string note, string effect = "")
        {
            Name = name;
			DisplayName = displayName;
            ItemSprite = itemSprite;
            ItemType = itemType;
            Cost = cost;
            Note = note;
			Effect = effect;
        }
    }

    void Start()
    {
        ConstructStoreItems();
        GenerateUIItems();
        LoadPlayerAndMapSkin();
		LoadItemsInUse ();
		GenerateActiveSpecialsArray (activeSpecials);
		LoadSpecialsInUse ();
        LoadOwnedItems();
	}

    void GenerateUIItems()
    {
        foreach (StoreItem item in StoreItems)
        {
            bool owned = CheckIfItemAlreadyOwned(item.Name);
			MM.CreateShopItems(item.Name, item.DisplayName, item.ItemType, item.ItemSprite, item.Cost, item.Note, owned);
        }
    }

    void ConstructStoreItems()
    {
        ItemContainer ic = ItemContainer.Load(path);

        foreach (Item item in ic.itemsData)
        {
            Sprite item_sprite = Resources.Load<Sprite>("Sprites/" + item.itemSprite);
			StoreItems.Add(new StoreItem(item.name, item.displayName, item_sprite, item.itemType, item.cost, item.note, item.effect));
        }
    }

    
    public void BuyItem(string object_name, GameObject uiElement)
    {
        LoadOwnedItems();
        bool owned = false;

        //Checks if item already exsist
        owned = CheckIfItemAlreadyOwned(object_name);
        int id = ObjectId(object_name);

        if (owned == false)
        {
            
            LoadPlayerMoney();

			if (playerCoins >= StoreItems [id].Cost) {
				PlaySound(click);
				current_buying_id = id;
				currentUiElement = uiElement;
				MM.SetFocusedButton (currentUiElement);
				MM.OpenConfirmWindow (true);
			} else
				PlaySound (notEnoughMoney);
        }
        else
        {
            //Already owned item case for player skin
			PlaySound(click);
			if (StoreItems [id].ItemType == "player-skin" || StoreItems [id].ItemType == "map-skin") {
				if (StoreItems [id].ItemType == "player-skin") {
					currentPlayerSkinInUse = StoreItems [id].Name;
					SavePlayerSkin (currentPlayerSkinInUse);
					PassPlayerTextures (Resources.LoadAll ("Player/" + currentPlayerSkinInUse, typeof(Sprite)));
				} 
				else 
				{
					currentMapSkinInUse = object_name;
					SaveMapSkin (currentMapSkinInUse);
					PassMapTextures(Resources.LoadAll("Map/" + currentMapSkinInUse + "/Tiles", typeof(Sprite)),Resources.LoadAll("Map/" + currentMapSkinInUse + "/Blocks", typeof(Sprite)));
				}
				//Ugly way to keep track of 1 active object only! but hey it works fucking perfectly!
				foreach (GameObject go in MM.UIElements) {
					if (go.GetComponent<SetShopObjProperties> ().Type == StoreItems [id].ItemType) {
						if (go.GetComponent<SetShopObjProperties> ().Name == object_name) {
							go.GetComponent<SetShopObjProperties> ().SetInUse (true);
						} else
							go.GetComponent<SetShopObjProperties> ().SetInUse (false);
					}
				}
			} else if (StoreItems [id].ItemType == "special") {
				{ 
					//specials
					LoadActiveSpecials ();

					GenerateActiveSpecialsArray (activeSpecials);

					if (CheckIfAlreadyActive (object_name)) {
						uiElement.GetComponent<SetShopObjProperties> ().SetInUse (false);
						bag.EffectCheck (object_name, false);
						string newActiveSpecials = "";

						foreach (string active in activeSpecialsList) {
							if (active != object_name) {
								if (active != "") {
									newActiveSpecials += active + ",";
								}
							}
						}
						SaveActiveSpecials (newActiveSpecials);
					} else if (CheckIfAlreadyActive (object_name) == false) {
						bag.EffectCheck (object_name, true);
						uiElement.GetComponent<SetShopObjProperties> ().SetInUse (true);
						activeSpecials += (object_name + ",");
						SaveActiveSpecials (activeSpecials);
					}
					LoadActiveSpecials ();
					GenerateActiveSpecialsArray (activeSpecials);
				}
			}
        }
    }

    public void ConfirmPurchease(bool condition)
    {
        if (condition)
        {
            LoadOwnedItems();
            playerCoins -= StoreItems[current_buying_id].Cost;
            ownedItems += "," + StoreItems[current_buying_id].Name;
            SavePlayerMoney();
            SaveOwnedItems();
            currentUiElement.GetComponent<SetShopObjProperties>().SetOwned();
			if (currentUiElement.GetComponent<SetShopObjProperties> ().Type == "player-item") 
			{
				currentUiElement.GetComponent<SetShopObjProperties> ().SetInUse (true);
				IncreaseStats (StoreItems [ObjectId (currentUiElement.GetComponent<SetShopObjProperties> ().Name)].Effect);
			}
			PlaySound (saleDone);
			MM.SetAlreadyOwnedButton (currentUiElement);
			MM.SetNewShopCoinsValue ();
        }
        else
        {
            current_buying_id = 0;
            currentUiElement = null;
			MM.UnselectAll ();
        }
        MM.OpenConfirmWindow(false);
    }

	bool CheckIfAlreadyActive(string object_name)
	{
		if (activeSpecialsList != null) 
		{
			if(activeSpecialsList.Contains(object_name))
				{
					return true;
				}
			}
				return false;
		}


    bool CheckIfItemAlreadyOwned(string object_name)
    {
        if (ownedItemsList != null)
        {
				if (ownedItemsList.Contains(object_name))
                {
                    return true;
                }
        }
        return false;

    }

    int ObjectId(string object_name)
    {
        int id = 0;

        foreach (StoreItem item in StoreItems)
        {
            if (item.Name == object_name)
            {
                return id;
            }
            id++;
        }

        return id;
    }

    void SavePlayerMoney()
    {
        PlayerPrefs.SetInt("Coins", playerCoins);
    }

    void LoadPlayerMoney()
    {
        playerCoins = PlayerPrefs.GetInt("Coins");
    }

    void SaveOwnedItems()
    {
        PlayerPrefs.SetString("OwnedItems", ownedItems);
        GenerateOwnedItemsArray(ownedItems);
    }
		

    void LoadOwnedItems()
    {
        ownedItems = PlayerPrefs.GetString("OwnedItems");
        GenerateOwnedItemsArray(ownedItems);

        if (CheckIfItemAlreadyOwned("DefaultPlayer") == false)
        {
            ownedItems = "DefaultPlayer," + ownedItems;
            SaveOwnedItems();
        }
        if (CheckIfItemAlreadyOwned("DefaultMap") == false)
        {
            ownedItems = "DefaultMap," + ownedItems;
            SaveOwnedItems();
        }
    }

    void SavePlayerSkin(string name)
    {
        PlayerPrefs.SetString("CurrentPlayerSkin", name);
    }

    void SaveMapSkin(string name)
    {
        PlayerPrefs.SetString("CurrentMapSkin", name);
    }

	void LoadActiveSpecials()
	{
		activeSpecials = PlayerPrefs.GetString("ActiveSpecials");
	}

	void SaveActiveSpecials(string currentActiveSpecials)
	{
		activeSpecials = currentActiveSpecials;
		PlayerPrefs.SetString("ActiveSpecials", activeSpecials);
	}

    void LoadPlayerAndMapSkin()
    {
        currentPlayerSkinInUse = PlayerPrefs.GetString("CurrentPlayerSkin");
        currentMapSkinInUse = PlayerPrefs.GetString("CurrentMapSkin");

		if (currentPlayerSkinInUse == "") 
		{
			SavePlayerSkin("DefaultPlayer");
		}
		if (currentPlayerSkinInUse == "") 
		{
			SaveMapSkin("DefaultMap");
		}

		LoadOwnedItems ();

            foreach (GameObject go in MM.UIElements)
            {
               
                if (go.GetComponent<SetShopObjProperties>().Type == "player-skin")
                {
                    string currentGoName = go.GetComponent<SetShopObjProperties>().Name;
                    if (currentGoName == currentPlayerSkinInUse)
                    {
                        go.GetComponent<SetShopObjProperties>().SetInUse(true);
                        //Loads current skin in use
						PassPlayerTextures (Resources.LoadAll ("Player/" + currentPlayerSkinInUse, typeof(Sprite)));
                    }
                    else
                        go.GetComponent<SetShopObjProperties>().SetInUse(false);
                }
                if (go.GetComponent<SetShopObjProperties>().Type == "map-skin")
                {
                    if (go.GetComponent<SetShopObjProperties>().Name == currentMapSkinInUse)
                    {
                        go.GetComponent<SetShopObjProperties>().SetInUse(true);
						//Pass map textures
					PassMapTextures(Resources.LoadAll("Map/" + currentMapSkinInUse + "/Tiles", typeof(Sprite)),Resources.LoadAll("Map/" + currentMapSkinInUse + "/Blocks", typeof(Sprite)));
                    }
                    else
                        go.GetComponent<SetShopObjProperties>().SetInUse(false);
                }
            }
    }

	void PassMapTextures(Object[] mapTiles, Object[] blockTiles)
	{
		{
			bag.mapTiles.Clear ();
			bag.blockTiles.Clear ();
			foreach (Sprite item in mapTiles)
			{
				bag.mapTiles.Add (item);
			}

			foreach (Sprite item in blockTiles)
			{
				bag.blockTiles.Add (item);
			}
		}
	}


	void PassPlayerTextures(Object[] playerSprites)
	{
		bag.playerSprites.Clear ();
		foreach (Sprite item in playerSprites)
		{
			bag.playerSprites.Add (item);
		}
	}
	void LoadItemsInUse()
	{
		bag.speedModifier = 0;
		bag.magnetModifier = 0;
		foreach (GameObject go in MM.UIElements) {

			if (go.GetComponent<SetShopObjProperties> ().Type == "player-item") 
			{
				string currentGoName = go.GetComponent<SetShopObjProperties>().Name;
				foreach (string currentOwnedItem in ownedItemsList) 
				{
					if (currentOwnedItem == currentGoName) {
						go.GetComponent<SetShopObjProperties>().SetInUse(true);
						string currentEffect = StoreItems [ObjectId (currentGoName)].Effect;
						IncreaseStats (currentEffect);
						break;
					}		
				}
			}
		}
	}

	void LoadSpecialsInUse()
	{
		foreach (GameObject go in MM.UIElements) {

			if (go.GetComponent<SetShopObjProperties> ().Type == "special") 
			{
				string currentGoName = go.GetComponent<SetShopObjProperties>().Name;
				if (activeSpecialsList.Contains (currentGoName)) {
					go.GetComponent<SetShopObjProperties>().SetInUse(true);
					bag.EffectCheck (currentGoName, true);
				}
			}
		}
	}

	void IncreaseStats(string effect)
	{
		if (effect == "speed") 
		{
			bag.speedModifier += 2;
		}
		else if (effect == "magnet") 
		{
			bag.magnetModifier += 1.5f;
		}
	}

    void Load()
    {
        LoadOwnedItems();
		LoadActiveSpecials ();
        LoadPlayerMoney();
    }

    void GenerateOwnedItemsArray(string ownedItems)
    {
			ownedItemsList = ownedItems.Split(',').ToList();
    }

	void GenerateActiveSpecialsArray(string activeSpecials)
	{
			activeSpecialsList = activeSpecials.Split (',').ToList();

	}

	void PlaySound(AudioClip clip)
	{
        if (!myAudioSource.isPlaying)
        {
            myAudioSource.clip = clip;
            myAudioSource.Play();
        }
	}
}
