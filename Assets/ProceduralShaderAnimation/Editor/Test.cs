using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(AnimationData))]
public class Test : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        var spawnPoint = new PropertyField(property.FindPropertyRelative("groupInfos"));
        root.Add(spawnPoint);

        var spawnInspector = new Box();
        root.Add(spawnInspector);
        
        spawnPoint.RegisterCallback<ChangeEvent<Object>, VisualElement>(
            SpawnChanged, spawnInspector);

        return root;
    }

    void SpawnChanged(ChangeEvent<Object> evt, VisualElement spawnInspector)
    {
        spawnInspector.Clear();

        var t = evt.newValue;
        if(t == null)
            return;
        
        spawnInspector.Add(new InspectorElement(t));
    }
}
