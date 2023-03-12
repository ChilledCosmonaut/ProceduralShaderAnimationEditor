using System.Collections.Generic;
using ProceduralShaderAnimation.ImageLogic;
using UnityEditor;
using UnityEngine;

namespace ProceduralShaderAnimation.Editor
{
    [CustomEditor(typeof(ShaderAnimationGenerator))]
    public class ShaderAnimationGeneratorEditor : UnityEditor.Editor
    {
        private ShaderAnimationGenerator generator;
        private AnimationData animationData;
        private GameObject boxPrefab, spherePrefab;

        [SerializeField] private GroupInfo currentActiveGroup;

        private GameObject boundingBox;
        private List<(GameObject, SphericalWeight)> sphereWeights;
        private List<(GameObject, RectangularWeight)> boxWeights;

        private bool recalculate = false;
        
        private void OnEnable()
        {
            generator = (ShaderAnimationGenerator) target;
            animationData = generator.animationData;
            currentActiveGroup = animationData.groupInfos[0];
            boxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ProceduralShaderAnimation/Prefabs/BoxWeight.prefab");
            spherePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ProceduralShaderAnimation/Prefabs/SphereWeight.prefab");
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
            foreach (FunctionData boxWeight in currentActiveGroup.weightInfos)
            {
                switch (boxWeight)
                {
                    case SplineWeight weight:
                        DrawSplineGizmo(weight);
                        break;
                }
            };
        }

        private void SetupGizmos()
        {
            DestroyGizmos();
            
            var bounds = generator.GetComponent<MeshFilter>().sharedMesh.bounds;
            
            SetupBoundingBox(bounds.center, bounds.extents);

            foreach (FunctionData boxWeight in currentActiveGroup.weightInfos)
            {
                switch (boxWeight)
                {
                    case RectangularWeight weight:
                        SetupBoxGizmo(weight);
                        break;
                    case SphericalWeight weight:
                        SetupSphereGizmo(weight);
                        break;
                }
            }
        }

        private void UpdateGizmos()
        {
            foreach ((GameObject gObject, SphericalWeight data) gizmoInfo in sphereWeights)
            {
                UpdateSphereGizmo(gizmoInfo.gObject, gizmoInfo.data);
            }
            
            foreach ((GameObject gObject, RectangularWeight data) gizmoInfo in boxWeights)
            {
                UpdateBoxGizmo(gizmoInfo.gObject, gizmoInfo.data);
            }
        }

        private void DestroyGizmos(){
            DestroyImmediate(boundingBox);   
        }

        private void SetupBoundingBox(Vector3 origin, Vector3 extents)
        {
            var boundingGizmo = Instantiate(boxPrefab, generator.transform);

            boundingGizmo.transform.position = origin;
            boundingGizmo.transform.localScale = extents;
            
            boundingBox = boundingGizmo;
        }
        
        private void SetupBoxGizmo(RectangularWeight weightInfo)
        {
            var boxGizmo = Instantiate(boxPrefab, boundingBox.transform);

            boxGizmo.transform.localPosition = weightInfo.origin - Vector3.one;
            boxGizmo.transform.localScale = weightInfo.diameters;
            boxGizmo.transform.hasChanged = false;
            
            boxWeights.Add((boxGizmo, weightInfo));
        }
        
        private void SetupSphereGizmo(SphericalWeight weightInfo)
        {
            var sphereGizmo = Instantiate(spherePrefab, boundingBox.transform);

            sphereGizmo.transform.position = weightInfo.origin - Vector3.one;
            sphereGizmo.transform.localScale = new Vector3(weightInfo.radius, weightInfo.radius, weightInfo.radius);
            sphereGizmo.transform.hasChanged = false;
            
            sphereWeights.Add((sphereGizmo, weightInfo));
        }

        private void DrawSplineGizmo(SplineWeight weightInfo)
        {
            Handles.SphereHandleCap(0, weightInfo.firstControlPoint, Quaternion.identity, 0.2f, EventType.Repaint);
            Handles.SphereHandleCap(0, weightInfo.secondControlPoint, Quaternion.identity, 0.15f, EventType.Repaint);
            Handles.DrawLine(weightInfo.firstControlPoint, weightInfo.secondControlPoint, 7);
            EditorGUI.BeginChangeCheck();
            Vector3 firstPoint = Handles.PositionHandle(weightInfo.firstControlPoint, Quaternion.identity);
            Vector3 secondPoint = Handles.PositionHandle(weightInfo.secondControlPoint, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(animationData, "Changed Spline Control Points");
                weightInfo.firstControlPoint = firstPoint;
                weightInfo.secondControlPoint = secondPoint;
                recalculate = true;
            }
        }

        private void UpdateBoxGizmo(GameObject gizmoObject, RectangularWeight data)
        {
            if (!gizmoObject.transform.hasChanged) return;
            
            Undo.RecordObject(generator.animationData, "Changed Box Weight");
            
            data.origin = gizmoObject.transform.position;
            data.diameters = gizmoObject.transform.localScale;
            
            gizmoObject.transform.hasChanged = false;
        }
        
        private void UpdateSphereGizmo(GameObject gizmoObject, SphericalWeight data)
        {
            if (!gizmoObject.transform.hasChanged) return;
            
            Undo.RecordObject(generator.animationData, "Changed Sphere Weight");
            
            data.origin = gizmoObject.transform.position;
            data.radius = gizmoObject.transform.localScale.x;
            
            gizmoObject.transform.hasChanged = false;
        }

    }
}
