using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [CreateAssetMenu(fileName = "PolynomialWeight", menuName = "ProceduralShaderAnimation/PolynomialWeight", order = 1)]
    public class PolynomialWeight : FunctionData
    {
        public Vector3 firstControlPoint, secondControlPoint;
        public float[] polynomialOrderPreambles;

        public override List<float>  GetDataAsFloatArray()
        {
            return new List<float>
            {  
                3,                           0,                    0,                    0,
                firstControlPoint.x,         firstControlPoint.y,  firstControlPoint.z,  0,
                secondControlPoint.x,        secondControlPoint.y, secondControlPoint.z, 0,
                polynomialOrderPreambles[0], 0,                    0,                    0,
                polynomialOrderPreambles[1], 0,                    0,                    0,
                polynomialOrderPreambles[2], 0,                    0,                    0,
                polynomialOrderPreambles[3], 0,                    0,                    0
            };
        }
    }
}