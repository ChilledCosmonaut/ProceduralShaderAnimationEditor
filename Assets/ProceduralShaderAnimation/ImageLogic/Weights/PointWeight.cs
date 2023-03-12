using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [CreateAssetMenu(fileName = "PointWeight", menuName = "ProceduralShaderAnimation/PointWeight", order = 1)]
    public class PointWeight : InterpolationData
    {
        public float firstWeight;
        public float secondWeight;

        public override List<float>  GetDataAsFloatArray()
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