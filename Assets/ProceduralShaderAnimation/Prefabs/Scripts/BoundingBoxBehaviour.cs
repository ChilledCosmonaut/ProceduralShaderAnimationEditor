using UnityEngine;

public class BoundingBoxBehaviour : MonoBehaviour
{
    public void OnDrawGizmos()
    {
        var boundingTransform = transform;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boundingTransform.position, boundingTransform.lossyScale * 2);
    }
}
 
 