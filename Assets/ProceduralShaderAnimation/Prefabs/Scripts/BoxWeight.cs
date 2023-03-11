using UnityEngine;

public class BoxWeight : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        var boundingTransform = transform;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boundingTransform.position, boundingTransform.lossyScale * 2);
    }
}
