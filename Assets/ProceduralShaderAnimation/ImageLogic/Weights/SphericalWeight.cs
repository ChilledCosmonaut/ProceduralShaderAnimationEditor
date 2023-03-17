using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class SphericalWeight : IData
    {
        public Vector3 origin;
        public float radius;

        public List<float>  GetDataAsFloatArray()
        {
            return new List<float>
            {  
                4,        0,        0,        0,
                origin.x, origin.y, origin.z, 0,
                radius,   0,        0,        0
            };
        }
    }
}