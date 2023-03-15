using UnityEditor;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Horn))]
public class Horn_Inspector : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create a new VisualElement to be the root the property UI
        var container = new VisualElement();

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SphereField.uxml");
        visualTree.CloneTree(container);
        
        // Return the finished UI
        return container;
    }
}
