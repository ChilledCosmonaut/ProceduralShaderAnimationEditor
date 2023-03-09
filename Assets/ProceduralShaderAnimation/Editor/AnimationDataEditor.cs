using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEngine;

namespace ProceduralShaderAnimation.Editor
{
    [CustomEditor(typeof(AnimationData))]
    public class AnimationDataEditor : UnityEditor.Editor
    {
        private AnimationData animationData;
    
        private void OnEnable()
        {
            // Method 1
            animationData = (AnimationData) target;
        }

        public override void OnInspectorGUI()
        {
            // Draw default inspector after button...
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Generate ImageData"))
            {
                animationData.CreateAnimationTexture();
            }
        }
    }
}