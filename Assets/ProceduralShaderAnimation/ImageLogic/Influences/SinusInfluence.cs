using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class SinusInfluence : IFunctionData
    {
        public bool useTime;
        public bool useOffset;
        public float amplitude, frequency, bias;

        public List<float> GetDataAsFloatArray()
        {
            var floatArray = new List<float>
            {
                1,                         0,                           0, 0,
                Convert.ToSingle(useTime), Convert.ToSingle(useOffset), 0, 0,
                amplitude,                 0,                           0, 0,
                frequency,                 0,                           0, 0,
                bias,                      0,                           0, 0,
            };
            
            return floatArray;
        }
        public float CalculateYValue(float x)
        {
            return amplitude * math.sin(frequency * x) + bias;
        }
    }
}