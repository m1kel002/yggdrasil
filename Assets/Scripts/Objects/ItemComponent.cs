using System.Collections;
using UnityEngine;

public class ItemComponent: MonoBehaviour
{
    [SerializeField]
    private bool isPickable = false;

    [SerializeField]
    private string itemName;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private string description;


    public Item OnPickUp()
    {
        if (isPickable)
        {
            Debug.Log("Item picked up");
            // gameObject.SetActive(false);
            StartCoroutine(Death());
        }


        return new Item(itemName, icon, description);
    }

    private IEnumerator Death()
    {
        yield return null;
        Destroy(gameObject);
    }

}
