using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class SplineWeight : InterpolationData
    {
        public Vector2[] splinePoints;

        public override List<float>  GetDataAsFloatArray()
        {
            return new List<float>
            {  
                2,                    0,                    0,                    0,
                FirstControlPoint.x,  FirstControlPoint.y,  FirstControlPoint.z,  0,
                SecondControlPoint.x, SecondControlPoint.y, SecondControlPoint.z, 0,
                splinePoints[0].x,    splinePoints[0].y,    0,                    0,
                splinePoints[1].x,    splinePoints[1].y,    0,                    0,
                splinePoints[2].x,    splinePoints[2].y,    0,                    0,
                splinePoints[3].x,    splinePoints[3].y,    0,                    0
            };
        }
    }
}