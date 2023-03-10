using System;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    internal struct AnimationDataReference
    {
        public Vector3 BoundingOrigin;
        public Vector3 BoundingScale;
        public Texture2D AnimationTexture;
    }
    
    public class ShaderAnimationGenerator : MonoBehaviour
    {
        private static readonly int BoundingOrigin = Shader.PropertyToID("_boundingOrigin");
        private static readonly int BoundingScale = Shader.PropertyToID("_boundingScale");
        private static readonly int AnimationTexture = Shader.PropertyToID("_AnimationTexture");
        
        [SerializeField] private AnimationData animationData;
        
        public void SetAnimationInfo()
        {
            Bounds bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
            Material material = GetComponent<Renderer>().sharedMaterial;
            Vector3 extents = bounds.extents;

            if (extents.x == 0) extents.x = 1;
            if (extents.y == 0) extents.y = 1;
            if (extents.z == 0) extents.z = 1;
            
            material.SetVector(BoundingOrigin, bounds.center);
            material.SetVector(BoundingScale, extents);
            material.SetTexture(AnimationTexture, animationData.CreateAnimationTexture());
        } 
        
        public void PrintMeshInfo()
        {
            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
            
            Debug.Log($"Bounds extent: {mesh.bounds.extents}, Bounds Origin: {mesh.bounds.center}{Environment.NewLine} Vertices: {string.Join(",", mesh.vertices)}");
        }
    }
}
