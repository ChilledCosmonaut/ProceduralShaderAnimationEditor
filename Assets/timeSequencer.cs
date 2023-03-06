using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeSequencer : MonoBehaviour
{
    private Renderer thisRend;

    [SerializeField]
    private AnimationCurve curve;

    private Texture2D texture;

    private float time = 0;

    private static readonly int Delta = Shader.PropertyToID("_delta");

    // Start is called before the first frame update
    void Start()
    {
        thisRend = GetComponent<Renderer>();
        Debug.Log(curve.keys);
        texture.EncodeToEXR();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        thisRend.material.SetFloat(Delta, time / 1.5f);
    }
}
