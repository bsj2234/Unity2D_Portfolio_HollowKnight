using UnityEngine;

public class InstanceItemContainer : MonoBehaviour
{
    public CharmInstance item;

    public CharmInstance TakeItem()
    {
        Destroy(gameObject);
        return item;
    }
}