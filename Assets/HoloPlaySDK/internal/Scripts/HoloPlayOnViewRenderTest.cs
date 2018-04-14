//Copyright 2017 Looking Glass Factory Inc.
//All rights reserved.
//Unauthorized copying or distribution of this file, and the source code contained herein, is strictly prohibited.

using System.Collections;
using System.Collections.Generic;

using HoloPlaySDK;

using UnityEngine;

namespace HoloPlaySDK_Tests
{
    public class HoloPlayOnViewRenderTest : MonoBehaviour
    {
        //Make sure to subscribe when enabled and unsubscribe to prevent memory leaks
        void OnEnable()
        {
            HoloPlay.onViewRender += FlipCubeOnView;
        }

        void OnDisable()
        {
            HoloPlay.onViewRender -= FlipCubeOnView;
        }

        void FlipCubeOnView(int viewIndex)
        {
            transform.localEulerAngles = new Vector3((viewIndex / HoloPlay.Config.numViews) * 150, 0);
        }
    }
}