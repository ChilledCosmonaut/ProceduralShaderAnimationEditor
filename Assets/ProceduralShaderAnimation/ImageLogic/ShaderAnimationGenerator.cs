using UnityEditor;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    public class ShaderAnimationGenerator : MonoBehaviour
    {
        private static readonly int BoundingOrigin = Shader.PropertyToID("_boundingOrigin");
        private static readonly int BoundingScale = Shader.PropertyToID("_boundingScale");
        private static readonly int AnimationTexture = Shader.PropertyToID("_AnimationTexture");
        private static readonly int GroupOffset = Shader.PropertyToID("_groupOffset");

#if UNITY_EDITOR
        public bool debug, isNormal = true;

        [SerializeField]
        [HideInInspector]
        private Material materialBackUp;
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
#if UNITY_EDITOR
            if (!debug) return;
            
            material.SetFloat(GroupOffset, animationData.debugOffset);
#endif
        }

#if UNITY_EDITOR
        public void SwitchToDebugMaterial()
        {
            if (!isNormal) return;
            Material debugMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/ProceduralShaderAnimation/Objects/DebugMaterial.mat");
            materialBackUp = GetComponent<Renderer>().sharedMaterial;
            GetComponent<Renderer>().sharedMaterial = debugMaterial;
            isNormal = false;
        }
        
        public void SwitchToStandardMaterial()
        {
            if (isNormal) return;
            GetComponent<Renderer>().sharedMaterial = materialBackUp;
            isNormal = true;
        }
#endif
    }
}