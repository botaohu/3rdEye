using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//example script of how to use the in-editor depth tracking

namespace HoloPlaySDK
{
    [ExecuteInEditMode]
    public class editorMove : editorDepthTouchTarget
    {
        //commented this out temporarily! just to get rid of editorwarning for a release. there were no refs to it
        // int temp = 0;

        // Update is called once per frame
        public override void onDepthTouch(List<depthTouch> touches)
        {
            if (touches.Count > 0)
                transform.position = touches[0].getLocalPos();
        }
    }
}