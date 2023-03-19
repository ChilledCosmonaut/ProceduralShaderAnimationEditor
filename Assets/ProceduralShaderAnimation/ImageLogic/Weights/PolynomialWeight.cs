using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class PolynomialWeight : InterpolationData, IFunctionData
    {
        public string name;
        public List<float> polynomialOrderPreambles = new(){1};

        public PolynomialWeight(string name)
        {
            this.name = name;
        }

        public List<float>  GetDataAsFloatArray()
        {
            var floatArray = new List<float>
            {
                3,                              0,                           0,                    0,
                FirstControlPoint.x,            FirstControlPoint.y,         FirstControlPoint.z,  0,
                SecondControlPoint.x,           SecondControlPoint.y,        SecondControlPoint.z, 0,
                polynomialOrderPreambles.Count, 0,                           0,                    0
            };

            foreach (var point in polynomialOrderPreambles)
            {
                floatArray.Add(point);
                floatArray.Add(0);
                floatArray.Add(0);
                floatArray.Add(0);
            }

            return floatArray;
        }
        public float CalculateYValue(float x)
        {
            if (polynomialOrderPreambles.Count == 0) return 0;
            
            float offset = polynomialOrderPreambles[0];
            float result = offset;

            for(int currentOrder = 1; currentOrder <= polynomialOrderPreambles.Count; currentOrder++){
                float prefix = polynomialOrderPreambles[currentOrder];
                result += prefix * math.pow(x, currentOrder);
            }

            return result;
        }
        public string GetName()
        {
            return name;
        }
    }
}