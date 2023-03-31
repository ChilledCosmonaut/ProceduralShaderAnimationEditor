using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "ProceduralShaderAnimation/AnimationInfo", order = 1)]
public class AnimationData : ScriptableObject
{
    public Texture2D animationTexture;
    
    public float animationLength;
    public List<GroupInfo> groupInfos;
    
#if UNITY_EDITOR
    
    [CanBeNull]
    public IFunctionData functionPreview;
    
    [CanBeNull]
    public GroupInfo groupPreview;
    public int debugOffset;

    public Action onBoxesChanged;
    public Action onTextureChanged;
    
#endif
    
    public void UpdateAnimationTexture()
    {
        var contentLength = 0f;
        var floatLists = new List<List<float>>
        {
            new()
            {
                animationLength, 0, 0, 0,
                groupInfos.Count, 0, 0, 0
            }
        };

        foreach (var groupInfo in groupInfos)
        {
            var dataArray = groupInfo.GetDataAsFloatArray();
            contentLength += dataArray.Count;
            floatLists.AddRange(dataArray);
        }
        
        floatLists[0][4] = contentLength;

        int maxLength = 0;

        foreach (List<float> floatList in floatLists)
        {
            if (maxLength < floatList.Count) maxLength = floatList.Count;
        }

        foreach (List<float> floatList in floatLists)
        {
            while (floatList.Count < maxLength)
            {
                floatList.Add(0);
            }
        }

        var floatFlattenedList = new List<float>();
        
        foreach (List<float> floatList in floatLists)
        {
            floatFlattenedList.AddRange(floatList);
        }

        var imageData = floatFlattenedList.ToArray();
        
        GenerateAnimationTexture(imageData, floatLists);
        
#if UNITY_EDITOR
        onTextureChanged?.Invoke();
#endif
    }

    private void GenerateAnimationTexture(float[] imageData, List<List<float>> formatList)
    {
        string textureName = $"Assets/ProceduralShaderAnimation/AnimationTextures/{Guid.NewGuid()}.asset";
        if (animationTexture != null) textureName = AssetDatabase.GetAssetPath(animationTexture);
        
        animationTexture = new Texture2D(formatList[0].Count / 4, formatList.Count, TextureFormat.RGBAFloat, true);

        animationTexture.SetPixelData(imageData, 0);
        animationTexture.Apply(updateMipmaps: false);
        
        AssetDatabase.CreateAsset(animationTexture, textureName);
        animationTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(textureName);
    }
}

public interface IData
{
    public List<float>  GetDataAsFloatArray();
}

public interface IFunctionData : IData
{
    public float CalculateYValue(float x);

    public string GetName();
}
[Serializable]
public abstract class InterpolationData
{
    public Vector3 firstControlPoint = Vector3.forward;
    public Vector3 secondControlPoint = Vector3.zero;
}

[Flags]
[Serializable]
public enum TransformationType
{
    Translation, Rotation, Scale
}

[Serializable]
public class GroupInfo
{
    public string name;
    public TransformationType transformationType = TransformationType.Translation;
    public Vector3 transformationAxis = Vector3.forward;
    public Vector3 offsetAxis = Vector3.forward;
    
    // Weight Lists...
    [SerializeReference]
    public List<IData> weights = new();
    
    // Influence Lists...
    [SerializeReference]
    public List<IData> influences = new();

    public GroupInfo(string name)
    {
        this.name = name;
    }

    public List<List<float>> GetDataAsFloatArray()
    {
        var floatArray = new List<List<float>>
        {
            new()
            {
                (float)transformationType + 1, 0,                    0,                    0,
                transformationAxis.x,      transformationAxis.y, transformationAxis.z, 0,
                offsetAxis.x,              offsetAxis.y,         offsetAxis.z,         0,
                weights.Count,             0,                    0,                    0,
                influences.Count,          0,                    0,                    0
            }
        };
        
        floatArray.AddRange(weights.Select(weightInfo => weightInfo.GetDataAsFloatArray()));
        floatArray.AddRange(influences.Select(influenceInfo => influenceInfo.GetDataAsFloatArray()));
        
        return floatArray;
    }
}