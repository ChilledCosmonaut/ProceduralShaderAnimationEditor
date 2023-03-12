using System.Collections.Generic;
using System.Linq;
using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ProceduralShaderAnimation.Editor
{
    [CustomEditor(typeof(ShaderAnimationGenerator))]
    public class ShaderAnimationGeneratorEditor : UnityEditor.Editor
    {
        private ShaderAnimationGenerator generator;
        private AnimationData animationData;
        [SerializeField] 
        private GroupInfo currentActiveGroup;

        private readonly List<(BoxBoundsHandle, RectangularWeight)> boxWeights = new ();

        private bool debug = true;
        private bool recalculate;
        
        private void OnEnable()
        {
            generator = (ShaderAnimationGenerator) target;
            animationData = generator.animationData;
            currentActiveGroup = animationData.groupInfos[0];
            SetupGizmos();
        }

        void OnDisable()
        {
            DestroyGizmos();
        }

        public override void OnInspectorGUI()
        {
            // Draw default inspector after button...
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Apply Animation"))
            {
                generator.SetAnimationInfo();
            }
            
            if (GUILayout.Button("Print Mesh Info"))
            {
                generator.PrintMeshInfo();
            }
        }
        
        private void OnSceneGUI()
        {
            if (!debug) return;

            foreach (FunctionData typelessWeight in currentActiveGroup.weightInfos)
            {
                switch (typelessWeight)
                {
                    case InterpolationData weight:
                        DrawInterpolationHandle(weight);
                        break;
                    case SphericalWeight weight:
                        DrawSphereHandle(weight);
                        break;
                }
            }

            foreach ((BoxBoundsHandle handle, RectangularWeight info) boxWeight in boxWeights)
                DrawBoxHandle(boxWeight.handle, boxWeight.info);

            if (!recalculate) return;
            
            generator.SetAnimationInfo();
            recalculate = false;
        }

        private void SetupGizmos()
        {
            DestroyGizmos();

            foreach (var weight in currentActiveGroup.weightInfos.OfType<RectangularWeight>())
                SetupBoxGizmo(weight);
        }

        private void DestroyGizmos(){
            boxWeights.Clear();
        }
        
        private void SetupBoxGizmo(RectangularWeight weightInfo)
        {
            var newBoundHandle = new BoxBoundsHandle
            {
                center = weightInfo.origin - Vector3.one,
                size = weightInfo.diameters
            };
            
            boxWeights.Add((newBoundHandle, weightInfo));
        }
        
        private void DrawBoxHandle(BoxBoundsHandle boxHandle, RectangularWeight weightInfo)
        {
            boxHandle.center = weightInfo.origin;
            boxHandle.size = weightInfo.diameters;
            
            EditorGUI.BeginChangeCheck();
            Vector3 newOrigin = Handles.PositionHandle(weightInfo.origin, generator.transform.rotation);
            boxHandle.DrawHandle();
            if (!EditorGUI.EndChangeCheck()) return;
            
            Undo.RecordObject(animationData, "Changed Box Weight");
            weightInfo.origin = newOrigin;
            weightInfo.diameters = boxHandle.size;
            recalculate = true;
        }
        
        private void DrawSphereHandle(SphericalWeight weightInfo)
        {
            var rotation = generator.transform.rotation;
            
            EditorGUI.BeginChangeCheck();
            Vector3 newOrigin = Handles.PositionHandle(weightInfo.origin, rotation);
            float scale = Handles.RadiusHandle(rotation, weightInfo.origin, weightInfo.radius);
            if (!EditorGUI.EndChangeCheck()) return;
            
            Undo.RecordObject(animationData, "Changed Sphere Weight");
            weightInfo.origin = newOrigin;
            weightInfo.radius = scale;
            recalculate = true;
        }

        private void DrawInterpolationHandle(InterpolationData weightInfo)
        {
            var rotation = generator.transform.rotation;
            
            Handles.SphereHandleCap(0, weightInfo.firstControlPoint, rotation, 0.2f, EventType.Repaint);
            Handles.SphereHandleCap(0, weightInfo.secondControlPoint, rotation, 0.15f, EventType.Repaint);
            Handles.DrawLine(weightInfo.firstControlPoint, weightInfo.secondControlPoint, 7);
            
            EditorGUI.BeginChangeCheck();
            Vector3 firstPoint = Handles.PositionHandle(weightInfo.firstControlPoint, rotation);
            Vector3 secondPoint = Handles.PositionHandle(weightInfo.secondControlPoint, rotation);
            if (!EditorGUI.EndChangeCheck()) return;
            
            Undo.RecordObject(animationData, "Changed Spline Control Points");
            weightInfo.firstControlPoint = firstPoint;
            weightInfo.secondControlPoint = secondPoint;
            recalculate = true;
        }
    }
}
