using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class SphericalWeight : IData
    {
        public string name;
        public Vector3 origin = Vector3.one;
        public float radius = 1;
        
        public SphericalWeight(string name)
        {
            this.name = name;
        }

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