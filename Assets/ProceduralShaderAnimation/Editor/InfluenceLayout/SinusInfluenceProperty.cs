using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEngine.UIElements;

namespace ProceduralShaderAnimation.Editor.InfluenceLayout
{
    [CustomPropertyDrawer(typeof(SinusInfluence))]
    public class SinusInfluenceProperty : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyDrawer = new VisualElement();
        
            var box = new Box();
            propertyDrawer.Add(box);

            var layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ProceduralShaderAnimation/Editor/InfluenceLayout/SinusInfluenceLayout.uxml");
            layout.CloneTree(box);

            return propertyDrawer;
        }
    }
}