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
        
        private Material _material;
        [HideInInspector]
        [SerializeField]
        private AnimationDataReference _animationDataReference;

        private void Start()
        {
            _material = GetComponent<Renderer>().material;
            _material.SetVector(BoundingOrigin, _animationDataReference.BoundingOrigin);
            _material.SetVector(BoundingScale, _animationDataReference.BoundingScale);
            _material.SetTexture(AnimationTexture, _animationDataReference.AnimationTexture);
            
            Debug.Log(_material.GetVector(BoundingOrigin));
        }
        
        public void SetAnimationInfo()
        {
            Bounds bounds = GetComponent<MeshFilter>().sharedMesh.bounds;

            _animationDataReference = new AnimationDataReference()
            {
                BoundingOrigin = bounds.center,
                BoundingScale = bounds.extents,
                AnimationTexture = animationData.GetDataAsFloatArray()
            };
        } 
    }
}
