using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEngine;

namespace ProceduralShaderAnimation.Editor
{
    [CustomEditor(typeof(ShaderAnimationGenerator))]
    public class ShaderAnimationGeneratorEditor : UnityEditor.Editor
    {
        private ShaderAnimationGenerator generator;
    
        private void OnEnable()
        {
            generator = (ShaderAnimationGenerator) target;
        }

        public override void OnInspectorGUI()
        {
            // Draw default inspector after button...
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Create Grid"))
            {
                
            }
        }
    }
}
