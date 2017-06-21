using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;


public class Item{

    [XmlAttribute("name")]
    public string name;

	[XmlElement("DisplayName")]
	public string displayName;

    [XmlElement("ItemSprite")]
    public string itemSprite;

    [XmlElement("ItemType")]
    public string itemType;

    [XmlElement("Cost")]
    public int cost;

    [XmlElement("Note")]
    public string note;

	[XmlElement("Effect")]
	public string effect;

}
