using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class SplineInfluence : FunctionData
    {
        public bool useTime;
        public bool useOffset;
        public Vector2 firstSplinePoint;
        public Vector2 secondSplinePoint;
        public Vector2 thirdSplinePoint;
        public Vector2 fourthSplinePoint;

        public override List<float> GetDataAsFloatArray()
        {
            var floatArray = new List<float>
            {
                2,                         0,                           0, 0,
                Convert.ToSingle(useTime), Convert.ToSingle(useOffset), 0, 0,
                firstSplinePoint.x,        firstSplinePoint.y,          0, 0,
                secondSplinePoint.x,       secondSplinePoint.y,         0, 0,
                thirdSplinePoint.x,        thirdSplinePoint.y,          0, 0,
                fourthSplinePoint.x,       fourthSplinePoint.y,         0, 0
            };    
            
            return floatArray;
        }
    }
}