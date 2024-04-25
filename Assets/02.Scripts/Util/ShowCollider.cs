using UnityEngine;

public class ShowCollider : MonoBehaviour
{
    public Color color = Color.yellow;
    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}