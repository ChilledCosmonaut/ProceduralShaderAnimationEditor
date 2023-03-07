using UnityEngine;
using Environment = System.Environment;

namespace ProceduralShaderAnimation.ImageLogic
{
    public class BoundsChecker : MonoBehaviour
    {
        [SerializeField]
        private Mesh checkedMesh;
    
        public void PrintOutMeshData()
        {
            Debug.Log($"Vertex Data: {string.Join(", ", checkedMesh.vertices)},{Environment.NewLine}Bound Data: {checkedMesh.bounds}");
        }
    }
}
