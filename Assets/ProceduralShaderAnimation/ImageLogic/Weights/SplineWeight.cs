using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class SplineWeight : InterpolationData, IFunctionData
    {
        public string name;
        public Vector2 firstSplinePoint = Vector2.zero;
        public Vector2 secondSplinePoint = new(0.2f, 0.2f);
        public Vector2 thirdSplinePoint = new(0.8f, 0.8f);
        public Vector2 fourthSplinePoint = Vector2.one;
        
        public SplineWeight(string name)
        {
            this.name = name;
        }

        public List<float>  GetDataAsFloatArray()
        {
            return new List<float>
            {  
                2,                    0,                    0,                    0,
                firstControlPoint.x,  firstControlPoint.y,  firstControlPoint.z,  0,
                secondControlPoint.x, secondControlPoint.y, secondControlPoint.z, 0,
                firstSplinePoint.x,   firstSplinePoint.y,   0,                    0,
                secondSplinePoint.x,  secondSplinePoint.y,  0,                    0,
                thirdSplinePoint.x,   thirdSplinePoint.y,   0,                    0,
                fourthSplinePoint.x,  fourthSplinePoint.y,  0,                    0
            };
        }
        
        public Vector2 CalculateYValue(float x, float time)
        {
            float sanitizedX = x / time;
            float yValue = (math.pow(1 - sanitizedX,3) * firstSplinePoint + 3 * sanitizedX * math.pow(1 - sanitizedX, 2) * secondSplinePoint + 3 * math.pow(sanitizedX, 2) * (1 - sanitizedX) * thirdSplinePoint + math.pow(sanitizedX, 3) * fourthSplinePoint).y;
            return new Vector2(sanitizedX, yValue);
        }
        public string GetName()
        {
            return name;
        }
    }
}