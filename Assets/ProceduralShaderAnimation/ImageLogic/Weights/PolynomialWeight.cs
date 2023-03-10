using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    [CreateAssetMenu(fileName = "PolynomialWeight", menuName = "ProceduralShaderAnimation/PolynomialWeight", order = 1)]
    public class PolynomialWeight : FunctionData
    {
        public Vector3 firstControlPoint, secondControlPoint;
        public List<float> polynomialOrderPreambles;

        public override List<float>  GetDataAsFloatArray()
        {
            var floatArray = new List<float>
            {
                3,                              0,                           0,                    0,
                firstControlPoint.x,            firstControlPoint.y,         firstControlPoint.z,  0,
                secondControlPoint.x,           secondControlPoint.y,        secondControlPoint.z, 0,
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