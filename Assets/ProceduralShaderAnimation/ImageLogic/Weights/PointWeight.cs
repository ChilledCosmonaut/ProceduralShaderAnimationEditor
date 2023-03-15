using System;
using System.Collections.Generic;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class PointWeight : InterpolationData
    {
        public float firstWeight;
        public float secondWeight;

        public override List<float>  GetDataAsFloatArray()
        {
            return new List<float>
            {
                1, 0, 0, 0,
                FirstControlPoint.x, FirstControlPoint.y, FirstControlPoint.z, 0,
                firstWeight, 0, 0, 0,
                SecondControlPoint.x, SecondControlPoint.y, SecondControlPoint.z, 0,
                secondWeight, 0, 0, 0
            };
        }
    }
}