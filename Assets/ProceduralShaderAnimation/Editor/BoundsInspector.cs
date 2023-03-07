using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEngine;

namespace ProceduralShaderAnimation.Editor
{
    [CustomEditor(typeof(BoundsChecker))]
    public class BoundsInspector : UnityEditor.Editor
    {
        private BoundsChecker boundsChecker;
    
        private void OnEnable()
        {
            // Method 1
            boundsChecker = (BoundsChecker) target;
        }

        public override void OnInspectorGUI()
        {
            // Draw default inspector after button...
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Create Grid"))
            {
                boundsChecker.PrintOutMeshData();
            }
        }
    }
}
