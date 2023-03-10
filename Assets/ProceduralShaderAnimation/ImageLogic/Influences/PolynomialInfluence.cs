using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [CreateAssetMenu(fileName = "PolynomialInfluence", menuName = "ProceduralShaderAnimation/PolynomialInfluence", order = 1)]
    public class PolynomialInfluence : FunctionData
    {
        public List<float> polynomialOrderPreambles;
        public bool useTime, useOffset;

        public override List<float> GetDataAsFloatArray()
        {
            var floatArray = new List<float>
            {
                3,                              0,                           0, 0,
                Convert.ToSingle(useTime),      Convert.ToSingle(useOffset), 0, 0,
                polynomialOrderPreambles.Count, 0,                           0, 0
            };

            foreach (var point in polynomialOrderPreambles)
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