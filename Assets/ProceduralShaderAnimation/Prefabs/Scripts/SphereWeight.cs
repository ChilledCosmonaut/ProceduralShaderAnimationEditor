using UnityEngine;

public class SphereWeight : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        var boundingTransform = transform;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(boundingTransform.position, boundingTransform.lossyScale.x);
    }
}
