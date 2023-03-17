using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ProceduralShaderAnimation.Editor
{
    [CustomEditor(typeof(AnimationData))]
    public class AnimationDataInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement dataInspector = new VisualElement();

            var lengthProperty = new PropertyField(serializedObject.FindProperty("animationLength"));
            dataInspector.Add(lengthProperty);
            
            var groupInfoProperty = new PropertyField(serializedObject.FindProperty("groupInfos"));
            dataInspector.Add(groupInfoProperty);

            return dataInspector;
        }
    }
}