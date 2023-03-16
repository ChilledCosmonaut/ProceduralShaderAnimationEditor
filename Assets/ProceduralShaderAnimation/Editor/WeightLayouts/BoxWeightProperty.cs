using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEngine.UIElements;

namespace ProceduralShaderAnimation.Editor.WeightLayouts
{
    [CustomPropertyDrawer(typeof(RectangularWeight))]
    public class BoxWeightProperty : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyDrawer = new VisualElement();
        
            var box = new Box();
            propertyDrawer.Add(box);

            var layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ProceduralShaderAnimation/Editor/WeightLayouts/BoxWeightLayout.uxml");
            layout.CloneTree(box);

            return propertyDrawer;
        }
    }
}

