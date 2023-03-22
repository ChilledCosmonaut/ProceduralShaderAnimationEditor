using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class SinusInfluence : IFunctionData
    {
        public string name;
        public bool useTime;
        public bool useOffset = true;
        public float amplitude = 1, frequency = 1, bias = 1;
        
        public SinusInfluence(string name)
        {
            this.name = name;
        }

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
        public string GetName()
        {
            return name;
        }
    }
}