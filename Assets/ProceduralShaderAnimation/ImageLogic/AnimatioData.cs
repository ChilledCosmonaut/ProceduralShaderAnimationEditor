using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "ProceduralShaderAnimation", order = 1)]
public class AnimatioData : ScriptableObject
{
    public AnimationInfo animationInfo;
    public List<GroupInfo> groupInfos;
}

[Serializable]
public struct AnimationInfo{
    public float animationLength;
    public int contentLength;
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
    public List<WeightInfo> weightInfos;
    public List<InfluenceInfo> influenceInfos;
}

[Serializable]
public struct WeightInfo{

}

[Serializable]
public struct InfluenceInfo{

}