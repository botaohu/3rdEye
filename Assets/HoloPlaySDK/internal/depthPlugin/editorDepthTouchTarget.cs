using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class is intended as a base class
//inherit from this to get depth touch updates IN EDIT MODE
//you can then use your class to do things like move things in the editor
//using the finger tracking cam

namespace HoloPlaySDK
{
    [ExecuteInEditMode]
    public class editorDepthTouchTarget : MonoBehaviour
    {

        void Awake()   //to use your own OnEnable use:   protected override void Awake()  {base.Awake()}
        {
            depthPlugin._setEditorDepthEventTarget(this, true);
        }
        protected virtual void OnDisable() //to use your own OnDisable use:   protected override void OnDisable()  {base.OnDisable()}
        {
            depthPlugin._setEditorDepthEventTarget(this, false);
        }
        protected virtual void OnDestroy() //to use your own OnDestroy use:   protected override void OnDestroy()  {base.OnDestroy()}
        {
            depthPlugin._setEditorDepthEventTarget(this, false);
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
