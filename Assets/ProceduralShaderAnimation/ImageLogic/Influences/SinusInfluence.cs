using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [CreateAssetMenu(fileName = "SinusInfluence", menuName = "ProceduralShaderAnimation/SinusInfluence", order = 1)]
    public class SinusInfluence : FunctionData
    {
        public bool useTime;
        public bool useOffset;
        public float amplitude, frequency, bias;

        public override List<float> GetDataAsFloatArray()
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
    }
}