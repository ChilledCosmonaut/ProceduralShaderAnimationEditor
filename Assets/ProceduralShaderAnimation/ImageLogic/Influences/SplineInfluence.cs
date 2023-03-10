using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [CreateAssetMenu(fileName = "SplineInfluence", menuName = "ProceduralShaderAnimation/SplineInfluence", order = 1)]
    public class SplineInfluence : FunctionData
    {
        public bool useTime;
        public bool useOffset;
        public Vector2[] splinePoints;

        public override List<float> GetDataAsFloatArray()
        {
            var floatArray = new List<float>
            {
                1,                         0,                           0, 0,
                Convert.ToSingle(useTime), Convert.ToSingle(useOffset), 0, 0,
                splinePoints[0].x,         splinePoints[0].y,           0, 0,
                splinePoints[1].x,         splinePoints[1].y,           0, 0,
                splinePoints[2].x,         splinePoints[2].y,           0, 0,
                splinePoints[3].x,         splinePoints[3].y,           0, 0
            };      
            
            return floatArray;
        }
    }
}