using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.InteropServices;

public class grabTextureTest : MonoBehaviour
{

    public Material colorMat;
    public Material depthMat;

    public HoloPlaySDK.depthPlugin depth;
	//noMultithreadTest depth;

    void OnEnable()
    {
		if (colorMat)
			colorMat.mainTexture = depth.getColorTexture();
		if (depthMat)
			depthMat.mainTexture = depth.getDepthTexture();
    }

}
