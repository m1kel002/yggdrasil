using UnityEngine;

public class Item
{
    [SerializeField]
    private string itemName;
    
    public string ItemName
    {
        get { return itemName; }
        set { itemName = value; }
    }

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private string description;
    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public Item(string itemName, Sprite icon, string description)
    {
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
    }
    
    public Item(string itemName, string description)
    {
        this.itemName = itemName;
        this.description = description;
    }
}
