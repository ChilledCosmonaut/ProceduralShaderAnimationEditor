using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [CreateAssetMenu(fileName = "PolynomialInfluence", menuName = "ProceduralShaderAnimation/PolynomialInfluence", order = 1)]
    public class PolynomialInfluence : FunctionData
    {
        public List<float> controlPoints;

        public override List<float> GetDataAsFloatArray()
        {
            var floatArray = new List<float>{controlPoints.Count, 0, 0, 0};

            foreach (var point in controlPoints)
            {
                floatArray.Add(point);
                floatArray.Add(0);
                floatArray.Add(0);
                floatArray.Add(0);
            }

            return floatArray;
        }
    }
}