using System;
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
        
        private void OnEnable()
        {
            generator = (ShaderAnimationGenerator) target;
            animationData = generator.animationData;
            boxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ProceduralShaderAnimation/Prefabs/BoxWeight.prefab");
            spherePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ProceduralShaderAnimation/Prefabs/SphereWeight.prefab");
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
        
        private void OnSceneViewGUI(SceneView sv)
        {
        }

        private void SetupGizmos()
        {
            DestroyGizmos();
            
            var bounds = generator.GetComponent<MeshFilter>().sharedMesh.bounds;
            
            Transform boundingTransform = SetupBoundingBox(bounds.center, bounds.extents, generator.transform);

            foreach (var boxWeight in currentActiveGroup.weightInfos)
            {
                if(boxWeight is RectangularWeight) SetupBoxGizmo(boxWeight as RectangularWeight, boundingTransform);
                else if ()
            }
            
            foreach (SphericalWeight sphereWeight in currentActiveGroup.weightInfos)
            {
                SetupSphereGizmo(sphereWeight, boundingTransform);
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
            Destroy(boundingBox);   
        }


        private Transform SetupBoundingBox(Vector3 origin, Vector3 extents, Transform parentTransform)
        {
            var boundingGizmo = PrefabUtility.InstantiatePrefab(boxPrefab, parentTransform) as GameObject;
            boundingBox = boundingGizmo;
            
            if (boundingGizmo == null) return new RectTransform();

            boundingGizmo.transform.position = origin;
            boundingGizmo.transform.localScale = extents;

            return boundingGizmo.transform;
        }
        
        private void SetupBoxGizmo(RectangularWeight weightInfo, Transform parentTransform)
        {
            var boxGizmo = PrefabUtility.InstantiatePrefab(boxPrefab, parentTransform) as GameObject;
            boxWeights.Add((boxGizmo, weightInfo));
            
            if (boxGizmo == null) return;

            boxGizmo.transform.position = weightInfo.origin;
            boxGizmo.transform.localScale = weightInfo.diameters;
            boxGizmo.transform.hasChanged = false;
        }
        
        private void SetupSphereGizmo(SphericalWeight weightInfo, Transform parentTransform)
        {
            var sphereGizmo = PrefabUtility.InstantiatePrefab(spherePrefab, parentTransform) as GameObject;
            sphereWeights.Add((sphereGizmo, weightInfo));
            
            if (sphereGizmo == null) return;

            sphereGizmo.transform.position = weightInfo.origin;
            sphereGizmo.transform.localScale = new Vector3(weightInfo.radius, weightInfo.radius, weightInfo.radius);
            sphereGizmo.transform.hasChanged = false;
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
