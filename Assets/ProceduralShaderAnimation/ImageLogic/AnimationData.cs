using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProceduralShaderAnimation.ImageLogic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "ProceduralShaderAnimation/AnimationInfo", order = 1)]
public class AnimationData : ScriptableObject
{
    public float animationLength;
    public List<GroupInfo> groupInfos;
    
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

        Debug.Log(contentLength);
        
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

[Serializable]
public abstract class FunctionData
{
    public abstract List<float>  GetDataAsFloatArray();

    public abstract float CalculateYValue(float x);
}
[Serializable]
public abstract class InterpolationData : FunctionData
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
    public TransformationType transformationType;
    public Vector3 transformationAxis;
    public Vector3 offsetAxis;
    
    // Weight Lists...
    public List<SplineWeight> splineWeights;
    public List<PointWeight> pointWeights;
    public List<PolynomialWeight> polynomialWeights;
    public List<RectangularWeight> boxWeights;
    public List<SphericalWeight> sphereWeights;
    
    // Influence Lists...
    public List<SinusInfluence> sinusInfluences;
    public List<PolynomialInfluence> polynomialInfluences;
    public List<SplineInfluence> splineInfluences;

    public List<List<float>> GetDataAsFloatArray()
    {
        var weightCount = splineWeights.Count + pointWeights.Count + polynomialWeights.Count + boxWeights.Count +
                          sphereWeights.Count;

        var influenceCount = sinusInfluences.Count + polynomialInfluences.Count + splineInfluences.Count;
        
        var floatArray = new List<List<float>>
        {
            new()
            {
                (float)transformationType, 0, 0, 0,
                transformationAxis.x, transformationAxis.y, transformationAxis.z, 0,
                offsetAxis.x, offsetAxis.y, offsetAxis.z, 0,
                weightCount, 0, 0, 0,
                influenceCount, 0, 0, 0
            }
        };
        
        floatArray.AddRange(splineWeights.Select(weightInfo => weightInfo.GetDataAsFloatArray()));
        floatArray.AddRange(pointWeights.Select(weightInfo => weightInfo.GetDataAsFloatArray()));
        floatArray.AddRange(polynomialWeights.Select(weightInfo => weightInfo.GetDataAsFloatArray()));
        floatArray.AddRange(boxWeights.Select(weightInfo => weightInfo.GetDataAsFloatArray()));
        floatArray.AddRange(sphereWeights.Select(weightInfo => weightInfo.GetDataAsFloatArray()));
        
        floatArray.AddRange(sinusInfluences.Select(influenceInfo => influenceInfo.GetDataAsFloatArray()));
        floatArray.AddRange(polynomialInfluences.Select(influenceInfo => influenceInfo.GetDataAsFloatArray()));
        floatArray.AddRange(splineInfluences.Select(influenceInfo => influenceInfo.GetDataAsFloatArray()));

        return floatArray;
    }
}