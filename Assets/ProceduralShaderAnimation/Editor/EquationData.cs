using System.Collections.Generic;
using UnityEngine;

namespace ProceduralShaderAnimation.Editor
{
    public class EquationData
    {
        private readonly List<Vector2> points;
        private Vector2 xLimits;
        private Vector2 yLimits;

        public int Length => points.Count;
        public float XMin => xLimits.x;
        public float XMax => xLimits.y;
        public float YMin => yLimits.x;
        public float YMax => yLimits.y;

        public bool IsEmpty => points.Count <= 0;

        public EquationData()
        {
            points = new List<Vector2>();
            xLimits = new Vector2(float.NaN, float.NaN);
            yLimits = new Vector2(float.NaN, float.NaN);
        }

        public void Add(Vector2 point)
        {
            if (IsEmpty)
            {
                xLimits = new Vector2(point.x, point.x);
                yLimits = new Vector2(point.y, point.y);
            }
            else
            {
                xLimits.x = point.x < xLimits.x ? point.x : xLimits.x;
                xLimits.y = point.x > xLimits.y ? point.x : xLimits.y;

                yLimits.x = point.y < yLimits.x ? point.y : yLimits.x;
                yLimits.y = point.y > yLimits.y ? point.y : yLimits.y;
            }

            points.Add(point);
        }

        public Vector2 GetItem(int index) => points[index];

        public void Clear()
        {
            points.Clear();
            xLimits = new Vector2(float.NaN, float.NaN);
            yLimits = new Vector2(float.NaN, float.NaN);
        }
    }
}