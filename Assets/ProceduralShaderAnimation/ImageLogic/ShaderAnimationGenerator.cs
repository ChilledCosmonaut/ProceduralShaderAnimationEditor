using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    public class ShaderAnimationGenerator : MonoBehaviour
    {
        private static readonly int BoundingOrigin = Shader.PropertyToID("_boundingOrigin");
        private static readonly int BoundingScale = Shader.PropertyToID("_boundingScale");
        private static readonly int AnimationTexture = Shader.PropertyToID("_AnimationTexture");

#if UNITY_EDITOR
        public bool debug;
#endif

        [SerializeField] public AnimationData animationData;

        public void SetAnimationInfo()
        {
            Bounds bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
            Material material = GetComponent<Renderer>().sharedMaterial;
            float extents = Mathf.Max(Mathf.Max(bounds.extents.x, bounds.extents.y), bounds.extents.z);

            material.SetVector(BoundingOrigin, bounds.center);
            material.SetFloat(BoundingScale, extents);
            material.SetTexture(AnimationTexture, animationData.animationTexture);
        }
    }
}