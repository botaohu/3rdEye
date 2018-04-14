using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//inherit from this class to automatically receive touch events from the hypercube
//alternatively, you can foreach loop on input.frontTouchScreen.touches or input.backTouchScreen.touches

//if you are using depth touches use TODO write doc!!

namespace HoloPlaySDK
{
    public class depthTouchTarget : MonoBehaviour
    {
        protected virtual void OnEnable()   //to use your own OnEnable use:   protected override void OnEnable()  {base.OnEnable()}
        {
            depthPlugin._setDepthEventTarget(this, true);
        }
        protected virtual void OnDisable() //to use your own OnDisable use:   protected override void OnDisable()  {base.OnDisable()}
        {
            depthPlugin._setDepthEventTarget(this, false);
        }
        protected virtual void OnDestroy() //to use your own OnDestroy use:   protected override void OnDestroy()  {base.OnDestroy()}
        {
            depthPlugin._setDepthEventTarget(this, false);
        }

        //Depth touches.  These are touches detected in the air from a depth camera like the Intel RealSense
        public virtual void onDepthTouch(List<depthTouch> touches)
        {
        }

        public virtual void onNoDepthTouches()
        {
        }

        //sends a message if it is open or closed
        public virtual void onHandState(bool open)
        {
        }
    }
}
