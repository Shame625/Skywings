using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetShopObjProperties : MonoBehaviour {

    public string Name;
	public string DisplayName;
    public string Type;
    GameObject ImageObject;
    GameObject TextObject;
    GameObject inUseSpot;



    void Awake()
    {
        ImageObject = transform.GetChild(0).transform.GetChild(0).gameObject;
        inUseSpot = transform.GetChild(0).transform.GetChild(1).gameObject;
        TextObject = transform.GetChild(1).transform.GetChild(0).gameObject;
    }


    public void SetProperties(string name, string displayName, Sprite sprite, int cost, string note, bool owned, string type)
    {
        Name = name;
		DisplayName = displayName;

        Type = type;
        string myText;
        if (!owned)
        {
			myText = "Name: " + DisplayName + "\nCost: " + cost.ToString();
        }
        else
        {
			myText = "Name: " + DisplayName + "\nAlready owned!";
        }
        TextObject.GetComponent<Text>().text = myText;
        ImageObject.GetComponent<Image>().sprite = sprite;
    }

    public void Click()
    {
        GameObject.FindGameObjectWithTag("ShopManager").GetComponent<Shop>().BuyItem(Name, gameObject);
    }

    public void SetOwned()
    {
		string myText = "Name: " + DisplayName + "\nAlready owned!";
        TextObject.GetComponent<Text>().text = myText;
    }

    public void SetInUse(bool condition)
    {
        inUseSpot.SetActive(condition);
    }
}
