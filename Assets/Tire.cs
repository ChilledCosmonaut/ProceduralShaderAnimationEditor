using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Tire", menuName = "Tire", order = 1)]
public class Tire : ScriptableObject
{
    public float m_AirPressure = 21.5f;
    public int m_ProfileDepth = 4;
}
