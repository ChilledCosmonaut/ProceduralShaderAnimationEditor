using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "ProceduralShaderAnimation/AnimationInfo", order = 1)]
public class AnimationData : ScriptableObject
{
    public float animationLength;
    public List<GroupInfo> groupInfos;
    
#if UNITY_EDITOR
    
    [SerializeReference]
    public IFunctionData previewedFunction;
    [SerializeReference]
    public GroupInfo sceneDebugGroupInfo;
    
#endif
    
    public Texture2D CreateAnimationTexture()
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
        
        Texture2D tex = new(floatLists[0].Count / 4, floatLists.Count, TextureFormat.RGBAFloat, true);

        tex.SetPixelData(imageData, 0);
        tex.Apply(updateMipmaps: false);
        
        var image = tex.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);

        var dirPath = Application.dataPath + "/SaveImages/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".exr", image);
        
        return tex;
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
    public Vector3 FirstControlPoint, SecondControlPoint;
}

[Flags]
[Serializable]
public enum TransformationType
{
    Rotation, Translation, Scale
}

[Serializable]
public class GroupInfo
{
    public string name;
    public TransformationType transformationType;
    public Vector3 transformationAxis;
    public Vector3 offsetAxis;
    
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
                (float)transformationType, 0,                    0,                    0,
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