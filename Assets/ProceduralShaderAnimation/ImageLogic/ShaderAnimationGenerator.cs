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
            
            material.SetVector(BoundingOrigin, bounds.center);
            material.SetVector(BoundingScale, bounds.extents);
            material.SetTexture(AnimationTexture, animationData.CreateAnimationTexture());
        } 
        
        public void PrintMeshInfo()
        {
            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
            
            Debug.Log($"Bounds extent: {mesh.bounds.extents}, Bounds Origin: {mesh.bounds.center}{Environment.NewLine} Vertices: {string.Join(",", mesh.vertices)}");
        }
    }
}
