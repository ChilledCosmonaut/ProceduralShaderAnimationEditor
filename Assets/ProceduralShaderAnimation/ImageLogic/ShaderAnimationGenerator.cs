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
            Vector3 extents = bounds.extents;

            if (extents.x == 0) extents.x = 1;
            if (extents.y == 0) extents.y = 1;
            if (extents.z == 0) extents.z = 1;

            material.SetVector(BoundingOrigin, bounds.center);
            material.SetVector(BoundingScale, extents);
            material.SetTexture(AnimationTexture, animationData.animationTexture);
        }
    }
}