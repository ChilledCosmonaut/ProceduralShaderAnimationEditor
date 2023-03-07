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
    
    public float[] GetDataAsFloatArray()
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

        var stringifiedList = floatLists.Select(floatList => string.Join(", ", floatList)).ToList();

        var imageData = floatFlattenedList.ToArray();
        Debug.Log(string.Join(Environment.NewLine, stringifiedList));
        
        Texture2D tex = new(floatLists[0].Count / 4, floatLists.Count, TextureFormat.RGBAFloat, true);

        float[] test = {0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 0.2f, 0.1f};

        tex.SetPixelData(imageData, 0); // mip 0;
        tex.Apply(updateMipmaps: false);

        var image = tex.EncodeToEXR();

        Debug.Log(Application.dataPath);
        var dirPath = Application.dataPath + "/SaveImages/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".exr", image);
            
        //GetComponent<Renderer>().sharedMaterial.mainTexture = tex;
        
        return imageData;
    }
    
}

public abstract class FunctionData : ScriptableObject
{
    public abstract List<float>  GetDataAsFloatArray();
}

[Serializable]
public enum TransformationType
{
    Rotation, Translation, Scale
}

[Serializable]
public struct GroupInfo
{
    public TransformationType transformationType;
    public Vector3 transformationAxis;
    public List<FunctionData> weightInfos;
    public List<FunctionData> influenceInfos;
    
    public List<List<float>> GetDataAsFloatArray()
    {
        var floatArray = new List<List<float>>
        {
            new()
            {
                (float)transformationType, 0, 0, 0,
                transformationAxis.x, transformationAxis.y, transformationAxis.z, 0,
                weightInfos.Count, 0, 0, 0,
                influenceInfos.Count, 0, 0, 0
            }
        };
        
        floatArray.AddRange(weightInfos.Select(weightInfo => weightInfo.GetDataAsFloatArray()));
        floatArray.AddRange(influenceInfos.Select(influenceInfo => influenceInfo.GetDataAsFloatArray()));

        return floatArray;
    }
}