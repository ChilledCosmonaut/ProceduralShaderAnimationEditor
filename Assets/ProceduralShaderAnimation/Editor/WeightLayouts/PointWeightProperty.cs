using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEngine.UIElements;

namespace ProceduralShaderAnimation.Editor.WeightLayouts
{
    [CustomPropertyDrawer(typeof(PointWeight))]
    public class PointWeightProperty : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyDrawer = new VisualElement();
        
            var box = new Box();
            propertyDrawer.Add(box);

            var layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ProceduralShaderAnimation/Editor/WeightLayouts/PointWeightLayout.uxml");
            layout.CloneTree(box);

            return propertyDrawer;
        }
    }
}