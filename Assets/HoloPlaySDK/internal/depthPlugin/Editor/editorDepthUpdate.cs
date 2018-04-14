using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HoloPlaySDK
{
    [InitializeOnLoad]
    public class editorDepthUpdate
    {

        static depthPlugin d;

        static editorDepthUpdate()
        {
            //start();   //start automatically.
        }

        public static void start()
        {
            if (EditorApplication.isPlaying) //this is only for pure editor mode:  to allow updates so we can use the realsens to move things in the scene
                return;

            // // Set up callbacks.
            EditorApplication.update += editorUpdate;
            EditorApplication.playmodeStateChanged += editorUpdate;
        }

        //TODO - this is not connected to anything
        public static void stop()
        {
            EditorApplication.update -= editorUpdate;
            EditorApplication.playmodeStateChanged -= editorUpdate;
        }

        static void editorUpdate()
        {
            if (EditorApplication.isPlaying) //this is only for pure editor mode:  to allow updates so we can use the realsens to move things in the scene
                return;


            if (d)
            { 
                d.editorUpdate();
                return; //we're done.
            }

            //try to supply the depthPlugin
            d = depthPlugin.get(); //this doesn't usually work in-editor.
            if (!d)
                d = GameObject.FindObjectOfType<depthPlugin>();

            //try again.
            if (d)
                d.editorUpdate();
           // else
          //      stop();
        }
    }
}
