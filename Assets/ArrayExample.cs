using UnityEditor;
using UnityEngine;

public class ArrayExample : ScriptableObject
{
    public int[] m_data;

    [MenuItem("Example/SerializedProperty GetArrayElementAtIndex Example")]
    static void Example()
    {
        ArrayExample obj = ScriptableObject.CreateInstance<ArrayExample>();
        obj.m_data = new int[] { 3, 2, 1 };

        SerializedObject serializedObject = new SerializedObject(obj);
        SerializedProperty property = serializedObject.FindProperty("m_data");

        for (int i = 0; i < property.arraySize; i++)
        {
            SerializedProperty element = property.GetArrayElementAtIndex(i);

            //Will print
            //m_data.Array.data[0] : 3
            //m_data.Array.data[1] : 2
            //m_data.Array.data[2] : 1
            Debug.Log(element.propertyPath + " : " + element.intValue);
        }
    }
}
