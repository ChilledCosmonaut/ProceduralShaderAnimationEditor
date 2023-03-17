using ProceduralShaderAnimation.Editor;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ConcaveFill : EditorWindow
{
    private const float PaddingLeft = 15f;
    private const float PaddingRight = 15f;
    private const float PaddingTop = 15f;
    private const float PaddingBottom = 15f;
    private readonly FunctionData displayedFunction;

    public ConcaveFill(FunctionData function)
    {
        displayedFunction = function;
    }

    public void CreateGraphVisualization()
    {
        var canvas = new VisualElement();
        canvas.StretchToParentSize();
        canvas.generateVisualContent += DrawCanvas;
        rootVisualElement.Add(canvas);

        var xAxisLabel = new Label("0")
        {
            name = "xAxis",
            style = { position = Position.Absolute}
        };
        var yAxisLabel = new Label("2") { 
            name = "yAxis",
            style = { position = Position.Absolute} 
        };
        
        canvas.Add(xAxisLabel);
        canvas.Add(yAxisLabel);
    }

    void DrawCanvas(MeshGenerationContext ctx)
    {
        EquationData data = CalculateGraph();
        
        var painter = ctx.painter2D;
        painter.lineJoin = LineJoin.Round;
        painter.lineCap = LineCap.Round;

        var canvasRect = ctx.visualElement.layout;

        painter.strokeColor = Color.green;
        painter.lineWidth = 2;
        painter.BeginPath();

        float rectWidth = canvasRect.width - PaddingLeft - PaddingRight;
        float rectHeight = canvasRect.height - PaddingTop - PaddingBottom;
        float xAxisOffset = canvasRect.height * math.remap(data.YMin, data.YMax, 0, 1, 0);
        
        Vector2 startPoint = data.GetItem(0);
                
        float startXRemap = math.remap(data.XMin, data.XMax, 0, rectWidth, startPoint.x);
        float startYRemap = math.remap(data.YMin, data.YMax, 0, rectHeight, startPoint.y);
        var startResult = new Vector2(PaddingLeft + startXRemap, canvasRect.height - startYRemap - PaddingBottom);
        painter.MoveTo(startResult);
        
        for (int i = 1; i < 300; i++)
        { 
            Vector2 point = data.GetItem(i);
                
            float xRemap = math.remap(data.XMin, data.XMax, 0, rectWidth, point.x);
            float yRemap = math.remap(data.YMin, data.YMax, 0, rectHeight, point.y);
            var result = new Vector2(PaddingLeft + xRemap, canvasRect.height - yRemap - PaddingBottom);
            painter.LineTo(result);
        }
        painter.Stroke();

        painter.strokeColor = Color.white;
        painter.lineWidth = 5;
        // draw Y and X axis
        painter.BeginPath();
        painter.MoveTo(new Vector2(PaddingLeft, PaddingTop));
        painter.LineTo(new Vector2(PaddingLeft, canvasRect.height - PaddingBottom));
        painter.Stroke();
        painter.BeginPath();
        painter.LineTo(new Vector2(PaddingLeft, canvasRect.height - xAxisOffset - PaddingBottom));
        painter.LineTo(new Vector2(canvasRect.width - PaddingRight, canvasRect.height - xAxisOffset - PaddingBottom));
        painter.Stroke();

        var label = ctx.visualElement.Q<Label>("xAxis");

        label.style.left = 50;
    }

    private EquationData CalculateGraph()
    {
        var data = new EquationData();

        float xValue = 0;

        for (int i = 0; i < 300; i++)
        {
            data.Add(new Vector2(xValue,  displayedFunction.CalculateYValue(xValue)));
            xValue += 2.0f / 300.0f;
        }

        return data;
    }
}
