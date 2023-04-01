﻿using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [Serializable]
    public class PolynomialWeight : InterpolationData, IFunctionData
    {
        public string name;
        public List<float> polynomialOrderPreambles = new(){1};

        public PolynomialWeight(string name)
        {
            this.name = name;
        }

        public List<float>  GetDataAsFloatArray()
        {
            var floatArray = new List<float>
            {
                3,                              0,                           0,                    0,
                firstControlPoint.x,            firstControlPoint.y,         firstControlPoint.z,  0,
                secondControlPoint.x,           secondControlPoint.y,        secondControlPoint.z, 0,
                polynomialOrderPreambles.Count, 0,                           0,                    0
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
        public Vector2 CalculateYValue(float x, float time)
        {
            if (polynomialOrderPreambles.Count == 0) return Vector2.zero;
            
            float offset = polynomialOrderPreambles[0];
            float result = offset;

            for(int currentOrder = 1; currentOrder <= polynomialOrderPreambles.Count; currentOrder++){
                float prefix = polynomialOrderPreambles[currentOrder];
                result += prefix * math.pow(x, currentOrder);
            }

            return new Vector2(x, result);
        }
        public string GetName()
        {
            return name;
        }
    }
}