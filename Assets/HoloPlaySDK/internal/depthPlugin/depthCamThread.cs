using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

namespace HoloPlaySDK
{

    public class depthCamThread
    {


        [DllImport("depthPlugin")]
        private static extern void printDebug(bool offOn);

        [DllImport("depthPlugin")]
        public static extern bool hasDepthCamera(); //note that calling this will instantiate the plugin

        [DllImport("depthPlugin")]
        public static extern void setThreshold(float meters);

        [DllImport("depthPlugin")]
        static extern void init();//try to call this at least 60fps
        [DllImport("depthPlugin")]
        static extern void update();//try to call this at least 60fps
        [DllImport("depthPlugin")]
        static extern void shutDown();//destroy the realsense class inside the plugin

        [DllImport("depthPlugin")]
        static extern bool prepareTouchCollection();//call this first, returns false if the plugin is not properly loaded
        [DllImport("depthPlugin")]
        public static extern float getTouchData(); //then collect a float array piecemeal. 

        [DllImport("depthPlugin")]
        public static extern void setCalibration(float meterDiffNear, float zAverageNear, float meterDiffFar, float zAverageFar);


        private float distanceThresholdMeters = 1f;


        public depthCamThread(float _distanceThresholdMeters)
        {
            registerDebugCallback(new debugCallback(debugMethod)); //allow plugin to print to console
            //printDebug(_printDebug);

            init();

            distanceThresholdMeters = _distanceThresholdMeters;
        }


        ///////debug message handler
        private delegate void debugCallback(string message);
        [DllImport("depthPlugin")]
        private static extern void registerDebugCallback(debugCallback callback);

        private static void debugMethod(string message)
        {
            if (message.Contains("error"))
                shutDown();

            Debug.Log("HoloPlaySDK Depth: " + message);
        }
        ///////debug message handler


        // ------------------------------------------------------------------------
        // Invoked to indicate to this thread object that it should stop.
        // ------------------------------------------------------------------------
        private bool stopRequested = false;
        public void requestStop()
        {
            lock (this)
            {
                stopRequested = true;
            }
        }


        /// <summary>
        /// stop the thread.
        /// </summary>
        protected void stop()
        {
            stopRequested = true;
        }



        public void runForever()
        {
            // This try is for having a log message in case of an unexpected
            // exception.
            try
            {
                setThreshold(distanceThresholdMeters); //this calls '->get()' in the plugin, which causes the rs camera to be instantiated.

                while (!IsStopRequested())
                {
                    try
                    {
                        runOnce();
                    }
                    catch (System.Exception ioe)
                    {
                        Debug.LogWarning("Exception: " + ioe.Message + "\nStackTrace: " + ioe.StackTrace);
                    }
                }

                // Attempt to do a final cleanup. 
                shutDown();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Unknown exception: " + e.Message + " " + e.StackTrace);
                stopRequested = true;
                shutDown();
            }
        }


        // ------------------------------------------------------------------------
        // Just checks if 'RequestStop()' has already been called in this object.
        // ------------------------------------------------------------------------
        private bool IsStopRequested()
        {
            lock (this)
            {
                return stopRequested;
            }
        }

        // ------------------------------------------------------------------------
        // A single iteration of the semi-infinite loop. Attempt to read/write to
        // the serial device. If there are more lines in the queue than we may have
        // at a given time, then the newly read lines will be discarded. This is a
        // protection mechanism when the port is faster than the Unity program.
        // If not, we may run out of memory if the queue really fills.
        // ------------------------------------------------------------------------
        private void runOnce()
        {
            try
            {
                update();
                return;
            }
            catch (System.TimeoutException)
            {
                // This is normal, not everytime we have a report from the serial device
                return;
            }

        }

        public bool prepareTouches()
        {
            lock (this)
            {
                return prepareTouchCollection();
            }
        }


        //returns number of active touches
        public int getTouches(ref depthTouch[] touchPool)
        {
            if (touchPool == null)
                return 0;

            //move things to the touches, for demo
            int i = 0;
            float v = getTouchData();
            Vector3 pos;
            while (v != -9999f)
            {
                pos.x = v;
                pos.y = -getTouchData();
                pos.z = getTouchData();

                pos *= .01f;

                touchPool[i].setPosition(pos);
                i++;

                v = getTouchData(); //the next x
            }
            return i;
        }

    }
}