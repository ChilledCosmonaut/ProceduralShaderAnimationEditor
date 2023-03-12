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
        private Bounds objectBounds;

        private readonly List<(BoxBoundsHandle, RectangularWeight)> boxWeights = new ();

        private bool debug = true;
        private bool recalculate;
        
        private void OnEnable()
        {
            generator = (ShaderAnimationGenerator) target;
            animationData = generator.animationData;
            currentActiveGroup = animationData.groupInfos[0];
            objectBounds = generator.GetComponent<MeshFilter>().sharedMesh.bounds;
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
            Handles.color = Color.red;
            Handles.DrawWireCube(objectBounds.center, objectBounds.extents * 2);

            Handles.color = Color.white;
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
                center = TransformIntoWorldSpace(weightInfo.origin),
                size = TransformIntoWorldSpace(weightInfo.diameters)
            };
            
            boxWeights.Add((newBoundHandle, weightInfo));
        }
        
        private void DrawBoxHandle(BoxBoundsHandle boxHandle, RectangularWeight weightInfo)
        {
            boxHandle.center = TransformIntoWorldSpace(weightInfo.origin);
            boxHandle.size = TransformIntoWorldSpace(weightInfo.diameters);
            
            EditorGUI.BeginChangeCheck();
            Vector3 newOrigin = Handles.PositionHandle(boxHandle.center, generator.transform.rotation);
            boxHandle.DrawHandle();
            if (!EditorGUI.EndChangeCheck()) return;
            
            Undo.RecordObject(animationData, "Changed Box Weight");
            weightInfo.origin = TransformIntoBoundingSpace(newOrigin);
            weightInfo.diameters = TransformIntoBoundingSpace(boxHandle.size);
            recalculate = true;
        }
        
        private void DrawSphereHandle(SphericalWeight weightInfo)
        {
            var rotation = generator.transform.rotation;
            
            EditorGUI.BeginChangeCheck();
            Vector3 newOrigin = Handles.PositionHandle(TransformIntoWorldSpace(weightInfo.origin), rotation);
            float scale = Handles.RadiusHandle(rotation, TransformIntoWorldSpace(weightInfo.origin), weightInfo.radius * objectBounds.extents.x);
            if (!EditorGUI.EndChangeCheck()) return;
            
            Undo.RecordObject(animationData, "Changed Sphere Weight");
            weightInfo.origin = TransformIntoBoundingSpace(newOrigin);
            weightInfo.radius = scale / objectBounds.extents.x;
            recalculate = true;
        }

        private void DrawInterpolationHandle(InterpolationData weightInfo)
        {
            var rotation = generator.transform.rotation;

            var transformedFirstPoint = TransformIntoWorldSpace(weightInfo.firstControlPoint);
            var transformedSecondPoint = TransformIntoWorldSpace(weightInfo.firstControlPoint);
            
            Handles.SphereHandleCap(0, transformedFirstPoint, rotation, 0.2f, EventType.Repaint);
            Handles.SphereHandleCap(0, transformedSecondPoint, rotation, 0.15f, EventType.Repaint);
            Handles.DrawLine(transformedFirstPoint, transformedSecondPoint, 7);
            
            EditorGUI.BeginChangeCheck();
            Vector3 firstPoint = Handles.PositionHandle(transformedFirstPoint, rotation);
            Vector3 secondPoint = Handles.PositionHandle(transformedSecondPoint, rotation);
            if (!EditorGUI.EndChangeCheck()) return;
            
            Undo.RecordObject(animationData, "Changed Spline Control Points");
            weightInfo.firstControlPoint = TransformIntoBoundingSpace(firstPoint);
            weightInfo.secondControlPoint = TransformIntoBoundingSpace(secondPoint);
            recalculate = true;
        }

        private Vector3 TransformIntoBoundingSpace(Vector3 position)
        {
            return ScaleToBoundingSpace(position) - objectBounds.extents + objectBounds.center;
        }
        
        private Vector3 TransformIntoWorldSpace(Vector3 position)
        {
            return ScaleToWorldSpace(position) - objectBounds.extents + objectBounds.center;
        }
        
        private Vector3 ScaleToBoundingSpace(Vector3 position)
        {
            return new Vector3(position.x / objectBounds.extents.x, position.y / objectBounds.extents.y, position.z / objectBounds.extents.z);
        }
        
        private Vector3 ScaleToWorldSpace(Vector3 position)
        {
            return new Vector3(position.x * objectBounds.extents.x, position.y * objectBounds.extents.y, position.z * objectBounds.extents.z);
        }
    }
}
