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
        private static readonly int Delta = Shader.PropertyToID("_delta");

#if UNITY_EDITOR
        public bool debug, isNormal = true;

        [SerializeField]
        [HideInInspector]
        private Material materialBackUp;
#endif

        public AnimationData animationData;
        
        private Renderer materialRenderer;
        [SerializeField]
        private float time;
        public float TimeStep
        {
            get => time;
            set
            {
                time = value;
                if (animationData.animationLength != 0) time = value % animationData.animationLength;
            }
        }

        private void Start()
        {
            animationData.UpdateAnimationTexture();
            SetUniqueAnimationInfo();
            materialRenderer = GetComponent<Renderer>();
        }
        
        void Update()
        {
            TimeStep += Time.deltaTime;
            materialRenderer.material.SetFloat(Delta, TimeStep);
        }

        private void SetUniqueAnimationInfo()
        {
            Bounds bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
            Material material = GetComponent<Renderer>().material;
            float extents = Mathf.Max(Mathf.Max(bounds.extents.x, bounds.extents.y), bounds.extents.z);

            material.SetVector(BoundingOrigin, bounds.center);
            material.SetFloat(BoundingScale, extents);
            material.SetTexture(AnimationTexture, animationData.animationTexture);
        }

        public void SetSharedAnimationInfo()
        {
            Bounds bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
            Material material = GetComponent<Renderer>().sharedMaterial;
            float extents = Mathf.Max(Mathf.Max(bounds.extents.x, bounds.extents.y), bounds.extents.z);

            material.SetVector(BoundingOrigin, bounds.center);
            material.SetFloat(BoundingScale, extents);
            material.SetTexture(AnimationTexture, animationData.animationTexture);
#if UNITY_EDITOR
            material.SetFloat(Delta, TimeStep);
            
            if (!debug) return;
            material.SetFloat(GroupOffset, animationData.debugOffset);
#endif
        }

#if UNITY_EDITOR
        public void SwitchToDebugMaterial()
        {
            if (!isNormal) return;
            Material debugMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/ProceduralShaderAnimation/Shader/DebugMaterial.mat");
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