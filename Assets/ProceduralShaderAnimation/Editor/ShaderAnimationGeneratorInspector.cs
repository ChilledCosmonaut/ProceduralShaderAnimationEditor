using System.Collections.Generic;
using System.Linq;
using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using InspectorElement = UnityEditor.UIElements.InspectorElement;

namespace ProceduralShaderAnimation.Editor
{
    [CustomEditor(typeof(ShaderAnimationGenerator))]
    public class ShaderAnimationGeneratorInspector : UnityEditor.Editor
    {
        public VisualTreeAsset uxml;
        
        private ShaderAnimationGenerator generator;
        private AnimationData animationData;
        
        [SerializeField] 
        private GroupInfo currentActiveGroup;
        private Vector3 boundCenter;
        private float boundSize;

        private readonly List<(BoxBoundsHandle, RectangularWeight)> boxWeights = new ();

        private bool debug = true;
        private bool recalculate;
        
        private void OnEnable()
        {
            generator = (ShaderAnimationGenerator) target;
            animationData = generator.animationData;
            currentActiveGroup = animationData.groupInfos[0];
            var objectBounds = generator.GetComponent<MeshFilter>().sharedMesh.bounds;
            boundCenter = objectBounds.center;
            boundSize = Mathf.Max(Mathf.Max(objectBounds.extents.x, objectBounds.extents.y), objectBounds.extents.z);
            SetupGizmos();
        }

        void OnDisable()
        {
            DestroyGizmos();
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement myInspector = new VisualElement();

            var animationDataProperty = new PropertyField(serializedObject.FindProperty("animationData"));
            myInspector.Add(animationDataProperty);
            
            var animationDataInspector = new Box();
            myInspector.Add(animationDataInspector);

            animationDataProperty.RegisterCallback<ChangeEvent<Object>, VisualElement>(
                AnimationDataChanged, animationDataInspector);

            var debugInfoPreview = new Button
            {
                text = debug ? "Show Debug Info" : "Disable Debug Info"
            };
            myInspector.Add(debugInfoPreview);
            
            return myInspector;
        }
        
        void AnimationDataChanged(ChangeEvent<Object> evt, VisualElement transformInspector)
        {
            transformInspector.Clear();

            var t = evt.newValue;
            if (t == null)
                return;
        
            transformInspector.Add(new InspectorElement(t));
        }
        
        private void OnSceneGUI()
        {
            if (!debug) return;
            Handles.color = Color.red;
            Handles.DrawWireCube(boundCenter, boundSize * 2 * Vector3.one);

            Handles.color = Color.white;
            
            foreach (IData typelessWeight in currentActiveGroup.weights)
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

            foreach (var weight in currentActiveGroup.weights.Cast<RectangularWeight>())
            {
                SetupBoxGizmo(weight);
            }
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
            float scale = Handles.RadiusHandle(rotation, TransformIntoWorldSpace(weightInfo.origin), weightInfo.radius * boundSize);
            if (!EditorGUI.EndChangeCheck()) return;
            
            Undo.RecordObject(animationData, "Changed Sphere Weight");
            weightInfo.origin = TransformIntoBoundingSpace(newOrigin);
            weightInfo.radius = scale / boundSize;
            recalculate = true;
        }

        private void DrawInterpolationHandle(InterpolationData weightInfo)
        {
            var rotation = generator.transform.rotation;

            var transformedFirstPoint = TransformIntoWorldSpace(weightInfo.FirstControlPoint);
            var transformedSecondPoint = TransformIntoWorldSpace(weightInfo.SecondControlPoint);
            
            Handles.SphereHandleCap(0, transformedFirstPoint, rotation, 0.2f, EventType.Repaint);
            Handles.SphereHandleCap(0, transformedSecondPoint, rotation, 0.15f, EventType.Repaint);
            Handles.DrawLine(transformedFirstPoint, transformedSecondPoint, 7);
            
            EditorGUI.BeginChangeCheck();
            Vector3 firstPoint = Handles.PositionHandle(transformedFirstPoint, rotation);
            Vector3 secondPoint = Handles.PositionHandle(transformedSecondPoint, rotation);
            if (!EditorGUI.EndChangeCheck()) return;
            
            Undo.RecordObject(animationData, "Changed Spline Control Points");
            weightInfo.FirstControlPoint = TransformIntoBoundingSpace(firstPoint);
            weightInfo.SecondControlPoint = TransformIntoBoundingSpace(secondPoint);
            recalculate = true;
        }

        private Vector3 TransformIntoBoundingSpace(Vector3 position)
        {
            return ScaleToBoundingSpace(new Vector3(position.x + boundSize, position.y + boundSize, position.z + boundSize) - boundCenter);
        }
        
        private Vector3 TransformIntoWorldSpace(Vector3 position)
        {
            Vector3 displacedVector3 = ScaleToWorldSpace(position) + boundCenter;
            return new Vector3(displacedVector3.x - boundSize, displacedVector3.y - boundSize, displacedVector3.z - boundSize);
        }
        
        private Vector3 ScaleToBoundingSpace(Vector3 position)
        {
            return new Vector3(position.x / boundSize, position.y / boundSize, position.z / boundSize);
        }
        
        private Vector3 ScaleToWorldSpace(Vector3 position)
        {
            return new Vector3(position.x * boundSize, position.y * boundSize, position.z * boundSize);
        }
    }
}
