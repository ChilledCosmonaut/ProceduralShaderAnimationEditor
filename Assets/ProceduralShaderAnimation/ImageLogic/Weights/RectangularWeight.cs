using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class RectangularWeight : FunctionData
    {
        public Vector3 origin;
        public Vector3 diameters;

        public override List<float>  GetDataAsFloatArray()
        {
            return new List<float>
            {  
                5,             0,           0,           0,
                origin.x,      origin.y,    origin.z,    0,
                diameters.x,   diameters.y, diameters.z, 0
            };
        }
    }
}