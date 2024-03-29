using System.Collections.Generic;
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
        private ShaderAnimationGenerator generator;
        private AnimationData animationData;
        
        private Vector3 boundCenter;
        private float boundSize;

        private readonly List<(BoxBoundsHandle, RectangularWeight)> boxWeights = new ();
        private bool recalculate;
        
        private void OnEnable()
        {
            generator = (ShaderAnimationGenerator) target;
            animationData = generator.animationData;
            var objectBounds = generator.GetComponent<MeshFilter>().sharedMesh.bounds;
            boundCenter = objectBounds.center;
            boundSize = Mathf.Max(Mathf.Max(objectBounds.extents.x, objectBounds.extents.y), objectBounds.extents.z);
            SetupGizmos();
            animationData.onBoxesChanged += SetupGizmos;
            animationData.onTextureChanged += generator.SetSharedAnimationInfo;
        }

        void OnDisable()
        {
            animationData.onBoxesChanged -= SetupGizmos;
            animationData.onTextureChanged -= generator.SetSharedAnimationInfo;
            DestroyGizmos();
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement myInspector = new VisualElement();

            var animationDataProperty = new PropertyField(serializedObject.FindProperty("animationData"));
            myInspector.Add(animationDataProperty);

            PropertyField debugToggle = new PropertyField(serializedObject.FindProperty("debug"), "Show debug info");
            myInspector.Add(debugToggle);

            Slider floatSlider = new Slider("Current Time Step", 0, animationData.animationLength)
            {
                value = animationData.animationLength
            };
            myInspector.Add(floatSlider);
            
            floatSlider.RegisterCallback<ChangeEvent<float>>(UpdateTimeStep);
            
            debugToggle.RegisterCallback<ChangeEvent<bool>>(ToggleDebugMaterial);
            
            var animationDataInspector = new Box();
            myInspector.Add(animationDataInspector);

            animationDataProperty.RegisterCallback<ChangeEvent<Object>, VisualElement>(
                AnimationDataChanged, animationDataInspector);
            
            return myInspector;
        }

        private void UpdateTimeStep(ChangeEvent<float> evt)
        {
            generator.TimeStep = evt.newValue;
            generator.SetSharedAnimationInfo();
        }
        
        private void AnimationDataChanged(ChangeEvent<Object> evt, VisualElement transformInspector)
        {
            transformInspector.Clear();

            var t = evt.newValue;
            if (t == null)
                return;
        
            transformInspector.Add(new InspectorElement(t));
        }

        private void ToggleDebugMaterial(ChangeEvent<bool> toggleEvent)
        {
            bool toggleState = toggleEvent.newValue;

            if (toggleState)
            {
                generator.SwitchToDebugMaterial();
                return;
            }
            
            generator.SwitchToStandardMaterial();
        }
        
        private void OnSceneGUI()
        {
            if (!generator.debug) return;
            if(animationData.groupPreview?.weights == null) return;
            
            Handles.color = Color.red;
            Handles.DrawWireCube(boundCenter, boundSize * 2 * Vector3.one);

            Handles.color = Color.white;
            
            foreach (IData typelessWeight in animationData.groupPreview.weights)
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
            
            animationData.UpdateAnimationTexture();
            recalculate = false;
        }

        private void SetupGizmos()
        {
            DestroyGizmos();
            
            if(animationData.groupPreview?.weights == null) return;

            foreach (IData typelessWeight in animationData.groupPreview.weights)
            {
                if (typelessWeight is not RectangularWeight rectangularWeight) continue;
                SetupBoxGizmo(rectangularWeight);
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
                size = ScaleToWorldSpace(weightInfo.diameters * 2)
            };
            
            boxWeights.Add((newBoundHandle, weightInfo));
        }
        
        private void DrawBoxHandle(BoxBoundsHandle boxHandle, RectangularWeight weightInfo)
        {
            boxHandle.center = TransformIntoWorldSpace(weightInfo.origin);
            boxHandle.size = ScaleToWorldSpace(weightInfo.diameters * 2);
            
            EditorGUI.BeginChangeCheck();
            Vector3 newOrigin = Handles.PositionHandle(boxHandle.center, generator.transform.rotation);
            boxHandle.DrawHandle();
            if (!EditorGUI.EndChangeCheck()) return;
            
            Undo.RecordObject(animationData, "Changed Box Weight");
            weightInfo.origin = TransformIntoBoundingSpace(newOrigin);
            weightInfo.diameters = ScaleToBoundingSpace(boxHandle.size / 2);
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

            var transformedFirstPoint = TransformIntoWorldSpace(weightInfo.firstControlPoint);
            var transformedSecondPoint = TransformIntoWorldSpace(weightInfo.secondControlPoint);
            
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
