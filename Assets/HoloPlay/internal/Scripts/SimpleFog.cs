using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SimpleFog : MonoBehaviour
{
    [Range(0.001f, 1)]
    public float cutoff = 0.5f;
    Material mat;

    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        mat = new Material(Shader.Find("HoloPlay/Simple Fog"));
        UpdateMaterial();
    }

    void OnDisable()
    {
        DestroyImmediate(mat);
    }

    void Update()
    {
        UpdateMaterial();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, mat);
    }

    void UpdateMaterial()
    {
        if (mat != null)
        {
            mat.SetFloat("cutoff", cutoff);
        }
    }
}