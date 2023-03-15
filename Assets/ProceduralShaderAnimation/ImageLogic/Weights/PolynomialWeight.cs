using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class PolynomialWeight : InterpolationData
    {
        public List<float> polynomialOrderPreambles;

        public override List<float>  GetDataAsFloatArray()
        {
            var floatArray = new List<float>
            {
                3,                              0,                           0,                    0,
                FirstControlPoint.x,            FirstControlPoint.y,         FirstControlPoint.z,  0,
                SecondControlPoint.x,           SecondControlPoint.y,        SecondControlPoint.z, 0,
                polynomialOrderPreambles.Count, 0,                           0,                    0
            };
            
            Debug.Log(polynomialOrderPreambles.Count);

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