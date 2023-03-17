using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class SplineWeight : InterpolationData, IFunctionData
    {
        public Vector2 firstSplinePoint;
        public Vector2 secondSplinePoint;
        public Vector2 thirdSplinePoint;
        public Vector2 fourthSplinePoint;

        public List<float>  GetDataAsFloatArray()
        {
            return new List<float>
            {  
                2,                    0,                    0,                    0,
                FirstControlPoint.x,  FirstControlPoint.y,  FirstControlPoint.z,  0,
                SecondControlPoint.x, SecondControlPoint.y, SecondControlPoint.z, 0,
                firstSplinePoint.x,   firstSplinePoint.y,   0,                    0,
                secondSplinePoint.x,  secondSplinePoint.y,  0,                    0,
                thirdSplinePoint.x,   thirdSplinePoint.y,   0,                    0,
                fourthSplinePoint.x,  fourthSplinePoint.y,  0,                    0
            };
        }
        
        public float CalculateYValue(float x)
        {
            return (math.pow(1 - x,3) * firstSplinePoint + 3 * x * math.pow(1 - x, 2) * secondSplinePoint + 3 * math.pow(x, 2) * (1 - x) * thirdSplinePoint + math.pow(x, 3) * fourthSplinePoint).y;
        }
    }
}