using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEngine.UIElements;

namespace ProceduralShaderAnimation.Editor.WeightLayouts
{
    [CustomPropertyDrawer(typeof(SplineWeight))]
    public class SplineWeightProperty : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyDrawer = new VisualElement();
        
            var box = new Box();
            propertyDrawer.Add(box);

            var layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ProceduralShaderAnimation/Editor/WeightLayouts/SplineWeightLayout.uxml");
            layout.CloneTree(box);

            return propertyDrawer;
        }
    }
}