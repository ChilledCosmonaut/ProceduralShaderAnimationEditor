using System.Globalization;
using ProceduralShaderAnimation.Editor;
using Unity.Mathematics;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEngine;

public static class GraphDrawer
{
    private const float PaddingLeft = 10f;
    private const float PaddingRight = 2f;
    private const float PaddingTop = 15f;
    private const float PaddingBottom = 15f;

    public static void DrawGraph(Func<float, float, Vector2> function, EquationData evalData, Material mat, int evaluationSteps, float time)
        {
            Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
            if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(rect);
                GL.PushMatrix();

                GL.Clear(true, false, Color.black);
                mat.SetPass(0);
                
                EvaluateFunction(function, evalData, evaluationSteps, time);

                float rectWidth = rect.width - PaddingLeft - PaddingRight;
                float rectHeight = rect.height - PaddingTop - PaddingBottom;

                float baseValue = 0;

                if (evalData.YMin > 0) baseValue = evalData.YMin;
                else if (evalData.YMax < 0) baseValue = evalData.YMax;
                    
                float baseOffset = rectHeight * math.remap(evalData.YMin, evalData.YMax, 0, 1, baseValue);
                float maxValueOffset = rectHeight * math.remap(evalData.YMin, evalData.YMax, 0, 1, evalData.YMax);
                float minValueOffset = rectHeight * math.remap(evalData.YMin, evalData.YMax, 0, 1, evalData.YMin);

                // draw base graph
                GL.Begin(GL.LINES);
                GL.Color(new Color(1, 1, 1, 1));
                // draw Y axis
                GL.Vertex3(PaddingLeft, PaddingTop, 0);
                GL.Vertex3(PaddingLeft, rect.height - PaddingBottom, 0);
                // draw X axis
                GL.Vertex3(PaddingLeft, rect.height - baseOffset - PaddingBottom, 0);
                GL.Vertex3(rect.width - PaddingRight, rect.height - baseOffset - PaddingBottom, 0);
                GL.End();
                
                // draw graph
                GL.Begin(GL.LINE_STRIP);
                GL.Color(Color.cyan);
                for (int i = 0; i < evalData.Length; i++)
                {
                    Vector2 point = evalData.GetItem(i);

                    float x_remap = math.remap(evalData.XMin, evalData.XMax, 0, rectWidth, point.x);
                    float y_remap = math.remap(evalData.YMin, evalData.YMax, 0, rectHeight, point.y);

                    GL.Vertex3(PaddingLeft + x_remap, rect.height - y_remap - PaddingBottom, 0.0f);
                }
                GL.End();

                GL.PopMatrix();
                GUI.EndClip();

                // draw values
                float squareHeight = 10;
                float squareWidth = 20;
                //Numbers Base Line
                EditorGUI.LabelField(new Rect(rect.x + PaddingLeft - squareWidth, rect.y + rect.height - baseOffset - PaddingBottom + (squareHeight * 0.2f), squareWidth, squareHeight), baseValue.ToString(CultureInfo.InvariantCulture));
                EditorGUI.LabelField(new Rect(rect.x + rect.width - PaddingRight - squareWidth, rect.y + rect.height - baseOffset - PaddingBottom + (squareHeight * 0.2f), squareWidth, squareHeight), evalData.XMax.ToString("#.#")); // max lenght mark

                if (baseValue <= 0)
                    EditorGUI.LabelField(new Rect(rect.x + PaddingLeft - squareWidth, Mathf.Clamp(rect.y + rect.height - minValueOffset - PaddingBottom - squareHeight / 2, rect.y + PaddingTop, rect.y + rect.height - PaddingBottom), squareWidth, squareHeight), evalData.YMin.ToString("#.#")); // Min Label
                
                if (baseValue >= 0)
                    EditorGUI.LabelField(new Rect(rect.x + PaddingLeft - squareWidth, Mathf.Clamp(rect.y + rect.height - maxValueOffset - PaddingBottom - squareHeight / 2, rect.y + PaddingTop, rect.y + rect.height - PaddingBottom), squareWidth, squareHeight), evalData.YMax.ToString("#.#")); // Max Label
            }
        }
        
        private static void EvaluateFunction(Func<float, float, Vector2> function, EquationData evalData, int evaluationSteps, float time)
        {
            evalData.Clear();

            float xValue = 0;

            for (int i = 0; i < evaluationSteps; i++)
            {
                var resultData = function(xValue, time);
                evalData.Add(resultData);
                xValue += time / evaluationSteps;
            }
        }
}
