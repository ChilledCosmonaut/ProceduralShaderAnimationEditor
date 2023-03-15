using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(Car))]
public class Car_Inspector : Editor
{
    public VisualTreeAsset m_InspectorXML;
    
    
    private Car car;

    private void OnEnable()
    {
        car = (Car) target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        // Create a new VisualElement to be the root of our inspector UI
        VisualElement myInspector = new VisualElement();
        InspectorElement.FillDefaultInspector(myInspector, serializedObject, this);
        return myInspector;

        // Add a simple label
        /*myInspector.Add(new Label("This is a custom inspector"));

        // Load and clone a visual tree from UXML
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ProceduralShaderAnimation/Editor/Car_Inspector_UXML.uxml");
        visualTree.CloneTree(myInspector);#1#/*

        var tireProperty = serializedObject.FindProperty("m_Tires");

        var test = tireProperty.FindPropertyRelative("m_AirPressure");
        
        myInspector.Add(new PropertyField(test));#1#
        
        // Get a reference to the default inspector foldout control
        /*VisualElement inspectorFoldout = myInspector.Q("Default_Inspector");
        
        // Attach a default inspector to the foldout
        inspectorFoldout.Add(new InspectorElement(car.m_Tires));#1#
        
        // Return the finished inspector UI
        return myInspector;*/
    }
}
