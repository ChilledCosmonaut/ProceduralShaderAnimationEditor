using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class ConcaveFill : EditorWindow
{
    [MenuItem("Tools/Concave Fill")]
    public static void ShowExample()
    {
        ConcaveFill wnd = GetWindow<ConcaveFill>();
        wnd.titleContent = new GUIContent("Concave Fill");
    }

    public void CreateGUI()
    {
        var canvas = new VisualElement();
        canvas.StretchToParentSize();
        canvas.generateVisualContent += DrawCanvas;
        rootVisualElement.Add(canvas);
    }

    void DrawCanvas(MeshGenerationContext ctx)
    {
        var painter = ctx.painter2D;
        painter.fillColor = Color.red;
        painter.strokeColor = Color.black;
        painter.lineWidth = 10;
        painter.lineJoin = LineJoin.Round;

        /*painter.BeginPath();
        painter.MoveTo(new Vector2(100, 100));
        painter.LineTo(new Vector2(150, 150));
        painter.LineTo(new Vector2(200, 50));
        painter.ArcTo(new Vector2(300, 100), new Vector2(300,200), 100.0f);
        painter.LineTo(new Vector2(150, 250));
        painter.ClosePath();
        painter.Fill();
        painter.Stroke();*/

        var chart = new float[] {
            0.5f, 0.7f, 0.67f, 0.9f, 0.81f, 0.84f, 0.67f, 0.53f, 0.21f, 0.34f
        };

        var bezierPoints = new List<Vector2>()
        {
            new(100, 440), new (400, 230), new (500, 200)
        };

        var leftBottom = new Vector2(20, 200);
        var chartPos = new Vector2(20, 500);
        var chartWidth = 600;
        var chartHeight = 300.0f;

        painter.fillColor = Color.green;
        painter.BeginPath();
        painter.MoveTo(chartPos);
        painter.BezierCurveTo(bezierPoints[0], bezierPoints[1], bezierPoints[2]);
        /*float dx = chartWidth / chart.Length;
        float x = 0;
        foreach (var v in chart)
        {
            painter.LineTo(leftBottom + new Vector2(x, v * chartHeight));
            x += dx;
        }*//*
        painter.LineTo(new Vector2(chartWidth, chartPos.y));
        painter.ClosePath();*/
        painter.Stroke();
    }
}
