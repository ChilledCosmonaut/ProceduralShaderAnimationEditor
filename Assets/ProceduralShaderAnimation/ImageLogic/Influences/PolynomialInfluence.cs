using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class PolynomialInfluence : IFunctionData
    {
        public List<float> polynomialOrderPreambles;
        public bool useTime, useOffset;

        public List<float> GetDataAsFloatArray()
        {
            var floatArray = new List<float>
            {
                3,                              0,                           0, 0,
                Convert.ToSingle(useTime),      Convert.ToSingle(useOffset), 0, 0,
                polynomialOrderPreambles.Count, 0,                           0, 0
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
            float offset = polynomialOrderPreambles[0];
            float result = offset;

            for(int currentOrder = 1; currentOrder <= polynomialOrderPreambles.Count; currentOrder++){
                float prefix = polynomialOrderPreambles[currentOrder];
                result += prefix * math.pow(x, currentOrder);
            }

            return result;
        }
    }
}