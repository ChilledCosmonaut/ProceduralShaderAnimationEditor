using UnityEditor;
using UnityEngine.UIElements;
using PopupWindow = UnityEngine.UIElements.PopupWindow;

[CustomPropertyDrawer(typeof(GroupInfo))]
public class GroupPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var propertyDrawer = new VisualElement();
        
        var popup = new PopupWindow();
        propertyDrawer.Add(popup);
        popup.text = "Group Info";
        
        var layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ProceduralShaderAnimation/Editor/GroupLayout.uxml");
        layout.CloneTree(popup);

        return propertyDrawer;
    }
}
