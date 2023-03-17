using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEngine.UIElements;

namespace ProceduralShaderAnimation.Editor.InfluenceLayout
{
    [CustomPropertyDrawer(typeof(SplineInfluence))]
    public class SplineInfluenceProperty : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyDrawer = new VisualElement();
        
            var box = new Box();
            propertyDrawer.Add(box);

            var layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ProceduralShaderAnimation/Editor/InfluenceLayout/SplineInfluenceLayout.uxml");
            layout.CloneTree(box);

            return propertyDrawer;
        }
    }
}