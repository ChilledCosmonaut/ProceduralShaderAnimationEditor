using System;
using System.Collections.Generic;
using ProceduralShaderAnimation.ImageLogic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine.UIElements;

namespace ProceduralShaderAnimation.Editor.InfluenceLayout
{
    [CustomPropertyDrawer(typeof(PolynomialInfluence))]
    public class PolynomialInfluenceProperty : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyDrawer = new VisualElement();
        
            var box = new Box();
            propertyDrawer.Add(box);

            var layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ProceduralShaderAnimation/Editor/InfluenceLayout/PolynomialInfluenceLayout.uxml");
            layout.CloneTree(box);

            var orderValues = new List<float>();
            
            try
            {
                var polynomialOrderValues = property.FindPropertyRelative("polynomialOrderPreambles");
            
                for (int i = 0; i < polynomialOrderValues.arraySize; i++)
                {
                    orderValues.Add(polynomialOrderValues.GetArrayElementAtIndex(i).floatValue);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (orderValues.Count > 0)
                GraphDrawer.CreateGraphVisualization(box, (x) => CalculateYValue(x, orderValues));

            return propertyDrawer;
        }
        
        public float CalculateYValue(float x, List<float> orderValues)
        {
            float offset = orderValues[0];
            float result = offset;

            for(int currentOrder = 1; currentOrder <= orderValues.Count; currentOrder++){
                float prefix = orderValues[currentOrder];
                result += prefix * math.pow(x, currentOrder);
            }

            return result;
        }
    }
}