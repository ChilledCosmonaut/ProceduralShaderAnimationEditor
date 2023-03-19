using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class SplineInfluence : IFunctionData
    {
        public string name;
        public bool useTime;
        public bool useOffset;
        public Vector2 firstSplinePoint;
        public Vector2 secondSplinePoint;
        public Vector2 thirdSplinePoint;
        public Vector2 fourthSplinePoint;
        
        public SplineInfluence(string name)
        {
            this.name = name;
        }

        public List<float> GetDataAsFloatArray()
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
        
        public float CalculateYValue(float x)
        {
            return (math.pow(1 - x,3) * firstSplinePoint + 3 * x * math.pow(1 - x, 2) * secondSplinePoint + 3 * math.pow(x, 2) * (1 - x) * thirdSplinePoint + math.pow(x, 3) * fourthSplinePoint).y;
        }
        public string GetName()
        {
            return name;
        }
    }
}