using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [CreateAssetMenu(fileName = "RectangularWeight", menuName = "ProceduralShaderAnimation/RectangularWeight", order = 1)]
    public class RectangularWeight : FunctionData
    {
        public Vector3 origin;
        public Vector2 diameters;

        public override List<float>  GetDataAsFloatArray()
        {
            return new List<float>
            {  
                4,             0,           0,        0,
                origin.x,      origin.y,    origin.z, 0,
                diameters.x,   diameters.y, 0,        0
            };
        }
    }
}