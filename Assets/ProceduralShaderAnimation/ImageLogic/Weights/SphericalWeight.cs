using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [CreateAssetMenu(fileName = "SphericalWeight", menuName = "ProceduralShaderAnimation/SphericalWeight", order = 1)]
    public class SphericalWeight : FunctionData
    {
        public Vector3 origin;
        public float radius;

        public override List<float>  GetDataAsFloatArray()
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