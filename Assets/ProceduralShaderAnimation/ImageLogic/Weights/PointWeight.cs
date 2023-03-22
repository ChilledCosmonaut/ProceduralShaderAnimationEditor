using System;
using System.Collections.Generic;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class PointWeight : InterpolationData, IData
    {
        public string name;
        public float firstWeight = 1;
        public float secondWeight = 1;
        
        public PointWeight(string name)
        {
            this.name = name;
        }

        public List<float>  GetDataAsFloatArray()
        {
            return new List<float>
            {
                1, 0, 0, 0,
                firstControlPoint.x, firstControlPoint.y, firstControlPoint.z, 0,
                firstWeight, 0, 0, 0,
                secondControlPoint.x, secondControlPoint.y, secondControlPoint.z, 0,
                secondWeight, 0, 0, 0
            };
        }
    }
}