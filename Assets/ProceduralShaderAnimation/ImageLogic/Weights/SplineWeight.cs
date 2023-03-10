using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [CreateAssetMenu(fileName = "SplineWeight", menuName = "ProceduralShaderAnimation/SplineWeight", order = 1)]
    public class SplineWeight : FunctionData
    {
        public Vector3 firstControlPoint, secondControlPoint;
        public Vector2[] splinePoints;

        public override List<float>  GetDataAsFloatArray()
        {
            return new List<float>
            {  
                2,                    0,                    0,                    0,
                firstControlPoint.x,  firstControlPoint.y,  firstControlPoint.z,  0,
                secondControlPoint.x, secondControlPoint.y, secondControlPoint.z, 0,
                splinePoints[0].x,    splinePoints[0].y,    0,                    0,
                splinePoints[1].x,    splinePoints[1].y,    0,                    0,
                splinePoints[2].x,    splinePoints[2].y,    0,                    0,
                splinePoints[3].x,    splinePoints[3].y,    0,                    0
            };
        }
    }
}