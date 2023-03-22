using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ProceduralShaderAnimation.ImageLogic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace ProceduralShaderAnimation.Editor
{
    [CustomEditor(typeof(AnimationData))]
    public class AnimationDataInspector : UnityEditor.Editor
    {
        private const float defaultLenght = 2.0f;
        private const float defaultValue = 1.0f;

        private const float paddingLeft = 10f;
        private const float paddingRight = 2f;
        private const float paddingTop = 15f;
        private const float paddingBottom = 15f;

        private const int evaluationSteps = 300;

        private Material mat;

        private GUIStyle header;
        private AnimationData animationData;
        private EquationData evalData;

        private enum Types
        {
            Sinus, Polynomial, Spline, Point, Box, Sphere
        }

        private void OnEnable()
        {
            animationData = (AnimationData)target;
            var shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);

            evalData = new EquationData();
            header = new GUIStyle
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.white
                }
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            animationData.animationLength = EditorGUILayout.FloatField("Animation Length", animationData.animationLength);
            
            EditorGUI.indentLevel++;
            for (int groupIndex = 0; groupIndex < animationData.groupInfos.Count; groupIndex++)
            {
                DisplayGroup(animationData.groupInfos[groupIndex]);
            }
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Add Group"))
            {
                animationData.groupInfos.Add(new GroupInfo($"Group {animationData.groupInfos.Count}"));
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                animationData.UpdateAnimationTexture();
            }

            if (animationData.functionPreview == null) return;
            
            EditorGUILayout.LabelField($"Previewing: {animationData.functionPreview.GetName()}");
            
            TestDraw(animationData.functionPreview.CalculateYValue);
        }

        private GUIStyle CreateCustomBoxBackground(Color backgroundColor)
        {
            var consoleBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false); 
            consoleBackground.SetPixel(0, 0, backgroundColor);
            consoleBackground.Apply();
            
            return new GUIStyle(GUIStyle.none)
            {
                fontSize = 24,
                normal =
                {
                    textColor = Color.white,
                    background = consoleBackground
                },
                border =
                {
                    bottom = 2,
                    left = 2,
                    right = 2,
                    top = 2
                }
            };
        }

        private void DisplayGroup(GroupInfo groupInfo)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField(groupInfo.name, header);
            
            groupInfo.name = EditorGUILayout.TextField("Name", groupInfo.name);
            groupInfo.transformationType = (TransformationType)EditorGUILayout.EnumFlagsField("Transformation Type", groupInfo.transformationType);
            groupInfo.transformationAxis = EditorGUILayout.Vector3Field("Transformation Axis", groupInfo.transformationAxis);
            groupInfo.offsetAxis = EditorGUILayout.Vector3Field("Offset Axis", groupInfo.offsetAxis);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Weights", header);

            if (GUILayout.Button("Preview in scene"))
            {
                animationData.groupPreview = groupInfo;
                UpdateDebugOffset();
            }
            
            EditorGUI.indentLevel++;
            for (int weightIndex = 0; weightIndex < groupInfo.weights.Count; weightIndex++)
            {
                switch (groupInfo.weights[weightIndex])
                {
                    case SplineWeight weight:
                        DisplaySplineWeight(weight, groupInfo);
                        break;
                    case SphericalWeight weight:
                        DisplaySphereWeight(weight, groupInfo);
                        break;
                    case RectangularWeight weight:
                        DisplayBoxWeight(weight, groupInfo);
                        break;
                    case PolynomialWeight weight:
                        DisplayPolynomialWeight(weight, groupInfo);
                        break;
                    case PointWeight weight:
                        DisplayPointWeight(weight, groupInfo);
                        break;
                }
            }
            EditorGUI.indentLevel--;
            
            if (GUILayout.Button("Add new Weight"))
            {
                // create the menu and add items to it
                GenericMenu menu = new GenericMenu();

                // forward slashes nest menu items under submenus
                AddMenuItemForColor(menu, "Point Weight", (param) => AddWeight(param, groupInfo), Types.Point);
                AddMenuItemForColor(menu, "Polynomial Weight", (param) => AddWeight(param, groupInfo), Types.Polynomial);
                AddMenuItemForColor(menu, "Spline Weight", (param) => AddWeight(param, groupInfo), Types.Spline);
                AddMenuItemForColor(menu, "Box Weight", (param) => AddWeight(param, groupInfo), Types.Box);
                AddMenuItemForColor(menu, "Sphere Weight", (param) => AddWeight(param, groupInfo), Types.Sphere);

                // display the menu
                menu.ShowAsContext();
            }
            
            EditorGUILayout.Separator();
            
            EditorGUILayout.LabelField("Influences", header);
            EditorGUI.indentLevel++;
            for (int influenceIndex = 0; influenceIndex < groupInfo.influences.Count; influenceIndex++)
            {
                switch (groupInfo.influences[influenceIndex])
                {
                    case SplineInfluence weight:
                        DisplaySplineInfluence(weight, groupInfo);
                        break;
                    case PolynomialInfluence weight:
                        DisplayPolynomialInfluence(weight, groupInfo);
                        break;
                    case SinusInfluence weight:
                        DisplaySinusInfluence(weight, groupInfo);
                        break;
                }
            }
            EditorGUI.indentLevel--;
            
            if (GUILayout.Button("Add new Influence"))
            {
                // create the menu and add items to it
                GenericMenu menu = new GenericMenu();

                // forward slashes nest menu items under submenus
                AddMenuItemForColor(menu, "Spline Influence", (param) => AddInfluence(param, groupInfo), Types.Spline);
                AddMenuItemForColor(menu, "Polynomial Influence", (param) => AddInfluence(param, groupInfo), Types.Polynomial);
                AddMenuItemForColor(menu, "Sinus Influence", (param) => AddInfluence(param, groupInfo), Types.Sinus);

                // display the menu
                menu.ShowAsContext();
            }
            
            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Remove Group"))
            {
                animationData.groupInfos.Remove(groupInfo);
                UpdateDebugOffset();
            }
            
            GUILayout.EndVertical();
        }

        private void UpdateDebugOffset()
        {
            for (int groupIndex = 0; groupIndex < animationData.groupInfos.Count; groupIndex++)
            {
                if (animationData.groupPreview != animationData.groupInfos[groupIndex]) continue;

                animationData.debugOffset = groupIndex;
            }
            
            Debug.Log(animationData.debugOffset);
        }

        private void DisplaySplineWeight(SplineWeight weightInfo, GroupInfo owningGroup)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginVertical(GUI.skin.textArea);

            EditorGUILayout.LabelField(weightInfo.name, header);

            weightInfo.name = EditorGUILayout.TextField("Name", weightInfo.name);
            
            EditorGUILayout.Separator();
            
            weightInfo.firstControlPoint = EditorGUILayout.Vector3Field("First Control Point", weightInfo.firstControlPoint);
            weightInfo.secondControlPoint = EditorGUILayout.Vector3Field("Second Control Point", weightInfo.secondControlPoint);
            
            EditorGUILayout.Separator();
            
            weightInfo.firstSplinePoint = EditorGUILayout.Vector2Field("First Spline Point",   weightInfo.firstSplinePoint);
            weightInfo.secondSplinePoint = EditorGUILayout.Vector2Field("Second Spline Point", weightInfo.secondSplinePoint);
            weightInfo.thirdSplinePoint = EditorGUILayout.Vector2Field("Third Spline Point",   weightInfo.thirdSplinePoint);
            weightInfo.fourthSplinePoint = EditorGUILayout.Vector2Field("Fourth Spline Point", weightInfo.fourthSplinePoint);

            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Preview Graph"))
            {
                animationData.functionPreview = weightInfo;
            }
            
            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Delete Spline Weight"))
            {
                if (animationData.functionPreview == weightInfo)
                {
                    animationData.functionPreview = null;
                }
                owningGroup.weights.Remove(weightInfo);
            }

            GUILayout.EndVertical();
        }
        
        private void DisplaySphereWeight(SphericalWeight weightInfo, GroupInfo owningGroup)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField(weightInfo.name, header);

            weightInfo.name = EditorGUILayout.TextField("Name", weightInfo.name);
            
            EditorGUILayout.Separator();

            weightInfo.origin = EditorGUILayout.Vector3Field("Origin", weightInfo.origin);
            weightInfo.radius = EditorGUILayout.FloatField("Radius", weightInfo.radius);
            
            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Delete Sphere Weight"))
            {
                owningGroup.weights.Remove(weightInfo);
            }

            GUILayout.EndVertical();
        }
        
        private void DisplayBoxWeight(RectangularWeight weightInfo, GroupInfo owningGroup)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField(weightInfo.name, header);

            weightInfo.name = EditorGUILayout.TextField("Name", weightInfo.name);
            
            EditorGUILayout.Separator();

            weightInfo.origin = EditorGUILayout.Vector3Field("Origin", weightInfo.origin);
            weightInfo.diameters = EditorGUILayout.Vector3Field("Diameters", weightInfo.diameters);
            
            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Delete Box Weight"))
            {
                owningGroup.weights.Remove(weightInfo);
                animationData.onBoxesChanged();
            }

            GUILayout.EndVertical();
        }
        
        private void DisplayPolynomialWeight(PolynomialWeight weightInfo, GroupInfo owningGroup)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField(weightInfo.name, header);

            weightInfo.name = EditorGUILayout.TextField("Name", weightInfo.name);
            
            EditorGUILayout.Separator();

            weightInfo.firstControlPoint = EditorGUILayout.Vector3Field("First Control Point", weightInfo.firstControlPoint);
            weightInfo.secondControlPoint = EditorGUILayout.Vector3Field("Second Control Point", weightInfo.secondControlPoint);
            
            EditorGUILayout.Separator();

            for (int order = 0; order < weightInfo.polynomialOrderPreambles.Count; order++)
            {
                weightInfo.polynomialOrderPreambles[order] = EditorGUILayout.FloatField($"Preamble {order}", weightInfo.polynomialOrderPreambles[order]);
            }

            if (GUILayout.Button("Increase Order"))
            {
                weightInfo.polynomialOrderPreambles.Add(1);
            }
            
            if (GUILayout.Button("Decrease Order"))
            {
                weightInfo.polynomialOrderPreambles.RemoveAt(weightInfo.polynomialOrderPreambles.Count - 1);
            }

            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Preview Graph"))
            {
                animationData.functionPreview = weightInfo;
            }
            
            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Delete Polynomial Weight"))
            {
                owningGroup.weights.Remove(weightInfo);
            }

            GUILayout.EndVertical();
        }
        
        private void DisplayPointWeight(PointWeight weightInfo, GroupInfo owningGroup)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField(weightInfo.name, header);

            weightInfo.name = EditorGUILayout.TextField("Name", weightInfo.name);
            
            EditorGUILayout.Separator();

            weightInfo.firstControlPoint = EditorGUILayout.Vector3Field("First Control Point", weightInfo.firstControlPoint);
            weightInfo.secondControlPoint = EditorGUILayout.Vector3Field("Second Control Point", weightInfo.secondControlPoint);
            
            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Delete Point Weight"))
            {
                owningGroup.weights.Remove(weightInfo);
            }

            GUILayout.EndVertical();
        }
        
        private void DisplaySplineInfluence(SplineInfluence influenceInfo, GroupInfo owningGroup)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField(influenceInfo.name, header);

            influenceInfo.name = EditorGUILayout.TextField("Name", influenceInfo.name);
            
            EditorGUILayout.Separator();
            
            influenceInfo.useOffset = EditorGUILayout.Toggle("Use Offset", influenceInfo.useOffset);
            influenceInfo.useTime = EditorGUILayout.Toggle("Use Time", influenceInfo.useTime);
            
            EditorGUILayout.Separator();
            
            influenceInfo.firstSplinePoint = EditorGUILayout.Vector2Field("First Spline Point",   influenceInfo.firstSplinePoint);
            influenceInfo.secondSplinePoint = EditorGUILayout.Vector2Field("Second Spline Point", influenceInfo.secondSplinePoint);
            influenceInfo.thirdSplinePoint = EditorGUILayout.Vector2Field("Third Spline Point",   influenceInfo.thirdSplinePoint);
            influenceInfo.fourthSplinePoint = EditorGUILayout.Vector2Field("Fourth Spline Point", influenceInfo.fourthSplinePoint);

            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Preview Graph"))
            {
                animationData.functionPreview = influenceInfo;
            }
            
            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Delete Spline Influence"))
            {
                owningGroup.influences.Remove(influenceInfo);
            }

            GUILayout.EndVertical();
        }
        
        private void DisplayPolynomialInfluence(PolynomialInfluence influenceInfo, GroupInfo owningGroup)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField(influenceInfo.name, header);

            influenceInfo.name = EditorGUILayout.TextField("Name", influenceInfo.name);
            
            EditorGUILayout.Separator();
            
            influenceInfo.useOffset = EditorGUILayout.Toggle("Use Offset", influenceInfo.useOffset);
            influenceInfo.useTime = EditorGUILayout.Toggle("Use Time", influenceInfo.useTime);
            
            EditorGUILayout.Separator();

            for (int order = 0; order < influenceInfo.polynomialOrderPreambles.Count; order++)
            {
                influenceInfo.polynomialOrderPreambles[order] = EditorGUILayout.FloatField($"Preamble {order}", influenceInfo.polynomialOrderPreambles[order]);
            }

            if (GUILayout.Button("Increase Order"))
            {
                influenceInfo.polynomialOrderPreambles.Add(1);
            }
            
            if (GUILayout.Button("Decrease Order"))
            {
                influenceInfo.polynomialOrderPreambles.RemoveAt(influenceInfo.polynomialOrderPreambles.Count - 1);
            }

            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Preview Graph"))
            {
                animationData.functionPreview = influenceInfo;
            }
            
            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Delete Polynomial Influence"))
            {
                owningGroup.influences.Remove(influenceInfo);
            }

            GUILayout.EndVertical();
        }
        
        private void DisplaySinusInfluence(SinusInfluence influenceInfo, GroupInfo owningGroup)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField(influenceInfo.name, header);

            influenceInfo.name = EditorGUILayout.TextField("Name", influenceInfo.name);
            
            EditorGUILayout.Separator();
            
            influenceInfo.useOffset = EditorGUILayout.Toggle("Use Offset", influenceInfo.useOffset);
            influenceInfo.useTime = EditorGUILayout.Toggle("Use Time", influenceInfo.useTime);
            
            EditorGUILayout.Separator();
            
            influenceInfo.amplitude = EditorGUILayout.FloatField("Amplitude",   influenceInfo.amplitude);
            influenceInfo.frequency = EditorGUILayout.FloatField("Frequency", influenceInfo.frequency);
            influenceInfo.bias = EditorGUILayout.FloatField("Bias",   influenceInfo.bias);

            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Preview Graph"))
            {
                animationData.functionPreview = influenceInfo;
            }
            
            EditorGUILayout.Separator();
            
            if (GUILayout.Button("Delete Sinus Influence"))
            {
                owningGroup.influences.Remove(influenceInfo);
            }

            GUILayout.EndVertical();
        }
        
        // a method to simplify adding menu items
        void AddMenuItemForColor(GenericMenu menu, string menuPath, Action<object> action, Types type)
        {
            // the menu item is marked as selected if it matches the current value of m_Color
            menu.AddItem(new GUIContent(menuPath), false, param => action(param), type);
        }

        void AddWeight(object weightType, GroupInfo targetGroup)
        {
            switch ((Types) weightType)
            {
                case Types.Point:
                    targetGroup.weights.Add(new PointWeight($"Point Weight {targetGroup.weights.Count}"));
                    break;
                case Types.Polynomial:
                    targetGroup.weights.Add(new PolynomialWeight($"Polynomial Weight {targetGroup.weights.Count}"));
                    break;
                case Types.Spline:
                    targetGroup.weights.Add(new SplineWeight($"Spline Weight {targetGroup.weights.Count}"));
                    break;
                case Types.Box:
                    targetGroup.weights.Add(new RectangularWeight($"Box Weight {targetGroup.weights.Count}"));
                    animationData.onBoxesChanged();
                    break;
                case Types.Sphere:
                    targetGroup.weights.Add(new SphericalWeight($"Sphere Weight {targetGroup.weights.Count}"));
                    break;
            }
        }
        
        void AddInfluence(object weightType, GroupInfo targetGroup)
        {
            switch ((Types) weightType)
            {
                case Types.Spline:
                    targetGroup.influences.Add(new SplineInfluence($"Spline Influence {targetGroup.weights.Count}"));
                    break;
                case Types.Polynomial:
                    targetGroup.influences.Add(new PolynomialInfluence($"Polynomial Influence {targetGroup.weights.Count}"));
                    break;
                case Types.Sinus:
                    targetGroup.influences.Add(new SinusInfluence($"Sinus Influence {targetGroup.weights.Count}"));
                    break;
            }
        }

        private void GraphDrawer(Unity.Plastic.Newtonsoft.Json.Serialization.Func<float, float> function)
        {
            Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
            EditorGUI.IndentedRect(rect);
            Debug.Log($"Width:{rect.width}, Height:{rect.height}");
            if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(rect);
                GL.PushMatrix();

                GL.Clear(true, false, Color.black);
                mat.SetPass(0);

                float rectWidth = rect.width - paddingLeft - paddingRight;
                float rectHeight = rect.height - paddingTop - paddingBottom;

                float x_AxisOffset = rectHeight * math.remap(evalData.YMin, evalData.YMax, 0, 1, 0);
                float defaultValueOffset = rectHeight * math.remap(evalData.YMin, evalData.YMax, 0, 1, 1); ;

                // draw base graph
                GL.Begin(GL.LINES);
                GL.Color(new Color(1, 1, 1, 1));
                // draw Y axis
                GL.Vertex3(paddingLeft, paddingTop, 0);
                GL.Vertex3(paddingLeft, rect.height - paddingBottom, 0);
                // draw X axis
                GL.Vertex3(paddingLeft, rect.height - x_AxisOffset - paddingBottom, 0);
                GL.Vertex3(rect.width - paddingRight, rect.height - x_AxisOffset - paddingBottom, 0);
                // draw default values
                GL.Color(Color.green);
                GL.Vertex3(paddingLeft, Mathf.Clamp(rect.height - defaultValueOffset - paddingBottom, paddingTop, rect.height - paddingBottom), 0);
                GL.Vertex3(rect.width - paddingRight, Mathf.Clamp(rect.height - defaultValueOffset - paddingBottom, paddingTop, rect.height - paddingBottom), 0);
                GL.End();

                /*// evaluate func values
                /*if (evalData.IsEmpty)#1# CalculateGraph(function);

                // re-evaluate func values after input values changed
                /*if (f != f0 || z != z0 || r != r0)
                {
                    InitFunction();
                    EvaluateFunction();
                }#1#

                // draw graph
                GL.Begin(GL.LINE_STRIP);
                GL.Color(Color.cyan);
                for (int i = 0; i < evalData.Length; i++)
                {
                    Vector2 point = evalData.GetItem(i);

                    float x_remap = math.remap(evalData.XMin, evalData.XMax, 0, rectWidth, point.x);
                    float y_remap = math.remap(evalData.YMin, evalData.YMax, 0, rectHeight, point.y);

                    GL.Vertex3(paddingLeft + x_remap, rect.height - y_remap - paddingBottom, 0.0f);
                }
                GL.End();

                GL.PopMatrix();
                GUI.EndClip();

                // draw values
                float squareSize = 10;
                EditorGUI.LabelField(new Rect(rect.x + paddingLeft - squareSize, Mathf.Clamp(rect.y + rect.height - defaultValueOffset - paddingBottom - squareSize / 2, rect.y + paddingTop, rect.y + rect.height - paddingBottom), squareSize, squareSize), "1"); // heigt "1" mark
                EditorGUI.LabelField(new Rect(rect.x + paddingLeft - squareSize, rect.y + rect.height - x_AxisOffset - paddingBottom + (squareSize * 0.2f), squareSize, squareSize), "0"); // height "0" mark
                EditorGUI.LabelField(new Rect(rect.x + rect.width - paddingRight - squareSize, rect.y + rect.height - x_AxisOffset - paddingBottom + (squareSize * 0.2f), squareSize, squareSize), "2"); // max lenght mark*/
            }
        }
        
        private static EquationData CalculateGraph(Unity.Plastic.Newtonsoft.Json.Serialization.Func<float, float> function)
        {
            var data = new EquationData();

            float xValue = 0;

            for (int i = 0; i < 300; i++)
            {
                data.Add(new Vector2(xValue,  function(xValue)));
                xValue += 2.0f / 300.0f;
            }

            return data;
        }

        private void TestDraw(Unity.Plastic.Newtonsoft.Json.Serialization.Func<float, float> function)
        {
            Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
            if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(rect);
                GL.PushMatrix();

                GL.Clear(true, false, Color.black);
                mat.SetPass(0);

                float rectWidth = rect.width - paddingLeft - paddingRight;
                float rectHeight = rect.height - paddingTop - paddingBottom;

                float x_AxisOffset = rectHeight * math.remap(evalData.YMin, evalData.YMax, 0, 1, 0);
                float defaultValueOffset = rectHeight * math.remap(evalData.YMin, evalData.YMax, 0, 1, 1); ;

                // draw base graph
                GL.Begin(GL.LINES);
                GL.Color(new Color(1, 1, 1, 1));
                // draw Y axis
                GL.Vertex3(paddingLeft, paddingTop, 0);
                GL.Vertex3(paddingLeft, rect.height - paddingBottom, 0);
                // draw X axis
                GL.Vertex3(paddingLeft, rect.height - x_AxisOffset - paddingBottom, 0);
                GL.Vertex3(rect.width - paddingRight, rect.height - x_AxisOffset - paddingBottom, 0);
                // draw default values
                /*GL.Color(Color.green);
                GL.Vertex3(paddingLeft, Mathf.Clamp(rect.height - defaultValueOffset - paddingBottom, paddingTop, rect.height - paddingBottom), 0);
                GL.Vertex3(rect.width - paddingRight, Mathf.Clamp(rect.height - defaultValueOffset - paddingBottom, paddingTop, rect.height - paddingBottom), 0);*/
                GL.End();

                // evaluate func values
                if (evalData.IsEmpty) EvaluateFunction(function);

                // re-evaluate func values after input values changed
                EvaluateFunction(function);
                
                // draw graph
                GL.Begin(GL.LINE_STRIP);
                GL.Color(Color.cyan);
                for (int i = 0; i < evalData.Length; i++)
                {
                    Vector2 point = evalData.GetItem(i);

                    float x_remap = math.remap(evalData.XMin, evalData.XMax, 0, rectWidth, point.x);
                    float y_remap = math.remap(evalData.YMin, evalData.YMax, 0, rectHeight, point.y);

                    GL.Vertex3(paddingLeft + x_remap, rect.height - y_remap - paddingBottom, 0.0f);
                }
                GL.End();

                GL.PopMatrix();
                GUI.EndClip();

                // draw values
                float squareSize = 10;
                EditorGUI.LabelField(new Rect(rect.x + paddingLeft - squareSize, Mathf.Clamp(rect.y + rect.height - defaultValueOffset - paddingBottom - squareSize / 2, rect.y + paddingTop, rect.y + rect.height - paddingBottom), squareSize, squareSize), evalData.YMax.ToString("#.##")); // heigt "1" mark
                EditorGUI.LabelField(new Rect(rect.x + paddingLeft - squareSize, rect.y + rect.height - x_AxisOffset - paddingBottom + (squareSize * 0.2f), squareSize, squareSize), "0"); // height "0" mark
                EditorGUI.LabelField(new Rect(rect.x + rect.width - paddingRight - squareSize, rect.y + rect.height - x_AxisOffset - paddingBottom + (squareSize * 0.2f), squareSize, squareSize), evalData.XMax.ToString("#.##")); // max lenght mark
            }
        }
        
        private void EvaluateFunction(Unity.Plastic.Newtonsoft.Json.Serialization.Func<float, float> function)
        {
            evalData.Clear();

            float xValue = 0;

            for (int i = 0; i < evaluationSteps; i++)
            {
                evalData.Add(new Vector2(xValue,  function(xValue)));
                xValue += 1.0f / 300.0f;
            }
        }
    }
}