using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

using System.Runtime.InteropServices;


namespace HoloPlaySDK
{

    public class depthTouch
    {
        public depthTouch(int _id) { id = _id; }
        public int id { get; private set; }

        public void deactivate() { deactivated = true; }

        public void setPosition(Vector3 _p)
        {         
            if (deactivated)
            {
                deactivated = false;
                lastPos = _p;
            }
            else
            {
                lastPos = position;
            }
            position = _p;
        }
        public Vector3 getWorldPos(Transform camera)
        {
            return camera.TransformPoint(getLocalPos()); //TODO return hypercube.maincam
        }
        public Vector3 getLocalPos()
        {
            return position;
        }

        public Vector3 getLocalDiff()
        {
            return lastPos - position;
        }

        bool deactivated = false;

        Vector3 position;
        Vector3 lastPos;
    }

    //[ExecuteInEditMode]
    public class depthPlugin : MonoBehaviour
    {
   

        public bool debug;
        public float defaultThresholdInMeters = .5f;

        public bool convertDepthTexture = false;
        public float depthTextureThreshold = 4500f;


        //[Range(0f, 1f)]
        //public float touchLerp = .2f;
        [Tooltip("Higher value means more responsive but more noise, lower value means smoother touches but more latency.")]
        [Range(0f, 1f)]
        public float pluginLerp = .2f;

        [Tooltip("This is the threshold in centimeters at which when touches fall in our out of it will trigger open/close hand events.  Set at 0 to turn off these events.")]
        public float openThresholdCm = 2f;

        //public uint nearThreshold;
        //public uint flatnessThreshold;  //TODO implement these
        //public uint flatTestRadius;
        //public uint centerBias;



        /// <summary>
        /// Configure smoothness vs responsiveness.  Higher percent means more responsive, lower will be smoother, more interpolated input.
        /// </summary>
        /// <param name="percent"></param>
        [DllImport("depthPlugin")]
        public static extern void setTouchLerp(float percent);


        [DllImport("depthPlugin")]
        public static extern int getTouchPoolSize();


        /// These may crash or cause undefined behavior if the texture provided is not the proper format.
        /// called automatically when getDepthTexture() is called.
        [DllImport("depthPlugin")]
        static extern void setDepthTexture(System.IntPtr tex);
        [DllImport("depthPlugin")]
        static extern void updateDepthTexture();
        [DllImport("depthPlugin")]
        static extern void setColorTexture(System.IntPtr tex);
        [DllImport("depthPlugin")]
        static extern void updateColorTexture();

        /// <summary>
        /// get the x resolution of the depth camera
        /// </summary>
        [DllImport("depthPlugin")]
        public static extern int getDimX();
        /// <summary>
        /// get the y resolution of the depth camera
        /// </summary>
        [DllImport("depthPlugin")]
        public static extern int getDimY();


        public List<depthTouch> touches { get; private set; }
        public bool closedHand { get; private set; }
        depthTouch[] touchPool;

        private Thread thread;
        private depthCamThread pluginThread;

        Bounds touchBounds;
        Bounds lastFrameBounds;
        float lastTouchAngle;
        float lastSize;
        int lastFrameTouchCount;
        public float twist { get; private set; }
        public float pinch { get; private set; }
        public Vector3 averageNormalized { get; private set; }
        public Vector3 averageDiff { get; private set; }


        private static depthPlugin instance;
        public static depthPlugin get() { return instance; }


        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
           DontDestroyOnLoad(this.gameObject);
            //end singleton
        }

		void Start()
		{
			touchBounds = new Bounds();
			int size = getTouchPoolSize();
			touches = new List<depthTouch>();
			touchPool = new depthTouch[size];
			for (int i = 0; i < size; i++)
			{
				touchPool[i] = new depthTouch(i);
			}
		}


		void OnEnable()
		{
			pluginThread = new depthCamThread(defaultThresholdInMeters);
			setTouchLerp(pluginLerp);

		//	StartCoroutine("StartThread");
		//}

		//private IEnumerator StartThread()
		//{

			//yield return null; 

			thread = new Thread(new ThreadStart(pluginThread.runForever));
			thread.Start();

		}

        //these keep track of all depthTouchTargets, and hence the in input system can send them user input data as it is received.
        static HashSet<depthTouchTarget> eventTargets = new HashSet<depthTouchTarget>();
        public static void _setDepthEventTarget(depthTouchTarget t, bool addRemove)
        {
            if (addRemove)
                eventTargets.Add(t);
            else
                eventTargets.Remove(t);
        }
        //this handles the in-editor touches so we can use the same system to move things around in the editor
        static HashSet<editorDepthTouchTarget> editorEventTargets = new HashSet<editorDepthTouchTarget>();
        public static void _setEditorDepthEventTarget(editorDepthTouchTarget t, bool addRemove)
        {
            if (addRemove)
                editorEventTargets.Add(t);
            else
                editorEventTargets.Remove(t);
        }

        private static bool hadTouches = false;
        static void processDepthTouches(List<depthTouch> touches)
        {
#if UNITY_EDITOR
            if (eventTargets.Count == 0 && editorEventTargets.Count == 0)
                return;
#else
            if (eventTargets.Count == 0)
                return;
#endif

            if (touches.Count == 0)
            {

                if (hadTouches)
                {
                    foreach (depthTouchTarget target in eventTargets)
                        target.onNoDepthTouches();
#if UNITY_EDITOR
                    foreach (editorDepthTouchTarget target in editorEventTargets)
                        target.onNoDepthTouches();
#endif
                }

                hadTouches = false;
            }
            else
            {
                foreach (depthTouchTarget target in eventTargets)
                    target.onDepthTouch(touches);
#if UNITY_EDITOR
                foreach (editorDepthTouchTarget target in editorEventTargets)
                    target.onDepthTouch(touches);
#endif

                hadTouches = true;
            }
        }

        static void processDepthTouchOpenClose(bool open)
        {
            if (eventTargets.Count == 0)
                return;

            foreach (depthTouchTarget target in eventTargets)
                target.onHandState(open);
        }

        /// <summary>
        /// Are the depth touches within a given volume?
        /// </summary>
        /// <param name="pos">center of a volume to test</param>
        /// <param name="diameter">the diameter of the volume to test</param>
        /// <returns></returns>
        public bool isHolding(Vector3 pos, float diameter)
        {
            if (touches.Count < 0)
                return false;

            Bounds b = new Bounds(pos, new Vector3(diameter, diameter, diameter));

            foreach (depthTouch t in touches)
            {
                if (!b.Contains(t.getLocalPos()))
                    return false;
            }
            return true;
        }





        ///////////////////////////////////////
        /////////////////////////////////////// texture streaming code
        //https://forum.unity3d.com/threads/updating-a-texture-live-from-a-c-plugin.100333/

        protected Texture2D depthTexture = null;
        protected Color32[] depthPixels;
        protected GCHandle depthPixelsHandle;
        public Texture2D getDepthTexture()
        {
            if (depthTexture)
                return depthTexture;

            depthTexture = new Texture2D(getDimX(), getDimY(), TextureFormat.RGBA32, false);
            depthPixels = depthTexture.GetPixels32(0);
            depthPixelsHandle = GCHandle.Alloc(depthPixels, GCHandleType.Pinned);

            setDepthTexture(depthPixelsHandle.AddrOfPinnedObject());
            return depthTexture;
        }
        /// <summary>
        /// Once a depth texture has been requested, it will be updated indefinitely.
        /// Call this to reduce the performance hit if you are done using the depth texture.
        /// </summary>
        public void stopDepthStream()
        {
            if (depthPixelsHandle.IsAllocated)
                depthPixelsHandle.Free();

            depthPixels = null;
            depthTexture = null;
            setDepthTexture((System.IntPtr)0); //a null pointer
        }

        protected Texture2D colorTexture = null;
        protected Color32[] colorPixels;
        protected GCHandle colorPixelsHandle;
        public Texture2D getColorTexture()
        {
            if (colorTexture)
                return colorTexture;

            colorTexture = new Texture2D(getDimX(), getDimY(), TextureFormat.RGBA32, false);
            colorPixels = colorTexture.GetPixels32(0);
            colorPixelsHandle = GCHandle.Alloc(colorPixels, GCHandleType.Pinned);

            setColorTexture(colorPixelsHandle.AddrOfPinnedObject());
            return colorTexture;
        }
        /// <summary>
        /// Once a color texture has been requested, it will be updated indefinitely.
        /// Call this to reduce the performance hit if you are done using the color texture.
        /// </summary>
        public void stopColorStream()
        {
            if (colorPixelsHandle.IsAllocated)
                colorPixelsHandle.Free();

            colorPixels = null;
            colorTexture = null;         
            setDepthTexture((System.IntPtr)0); //a null pointer
        }



        void processTextureStreams()
        {
            if (depthTexture)
            {
                updateDepthTexture();  
                if (convertDepthTexture) 
                {
                    //converts the 16 bit depth data into a visual 8 bit grayscale texture
                    byte b;
                    for(int i = 0; i < depthPixels.Length; i++ )
                    {
                        b = utils.colorToDepth8(depthPixels[i], depthTextureThreshold);
                        depthPixels[i].r = b;
                        depthPixels[i].g = b;
                        depthPixels[i].b = b;
                    }
                }

                depthTexture.SetPixels32(depthPixels, 0);
                depthTexture.Apply(false, false);
            }
            if (colorTexture)
            {
                updateColorTexture();
                colorTexture.SetPixels32(colorPixels, 0);
                colorTexture.Apply(false, false);
            }
        }

        ///////////////////////////////////////
        ///////////////////////////////////////end texture streaming code


#if UNITY_EDITOR
        public void editorUpdate()
        {
            Update();
        }
#endif

        depthTouch mainTouchA = null;
        depthTouch mainTouchB = null;
        void Update() //rename his to getTouches?
        {
			if (pluginThread == null || !pluginThread.prepareTouches() || touches == null) //do this first.  if it gives false it means the plugin isn't even loaded.
                return;

            int count = pluginThread.getTouches(ref touchPool);
            touches.Clear();
            
            touchBounds = new Bounds();

            for (int i = 0; i < count; i ++)
            {
                if (touchPool[i].getLocalPos() != Vector3.zero)
                {
                    touchBounds.Encapsulate(touchPool[i].getLocalPos());
                    touches.Add(touchPool[i]);
                }
            }

            //handle open/closed hand
            if (lastFrameTouchCount > 1 && touches.Count == 1) //determine close
            {
                Vector3 testPos = touches[0].getLocalPos();
                testPos.z = lastFrameBounds.center.z; //invalidate testing the z;
                if (lastFrameBounds.Contains(testPos)) //if the single available touch is within the last  frame's touches bounds, call it a closed hand
                {
                    setHandState(false);
                }
            }
            else if (touches.Count > 2) //determine open
            {
                float openThresholdMeters = openThresholdCm * .01f;
                if ((touchBounds.size.x > openThresholdMeters) ||
                    (touchBounds.size.y > openThresholdMeters)) //note that Z is not tested here for determining open/close hand
                {
                    setHandState(true);
                }
            }


            //handle average positions
            averageNormalized = Vector3.zero;
            averageDiff = Vector3.zero;

            depthTouch highX = null;
            depthTouch lowX = null;
            depthTouch highY = null;
            depthTouch lowY = null;

            //main touches are a way for us to stabilize the twist and scale outputs by not hopping around different touches, instead trying to calculate the values from the same touches if possible
            bool updateMainTouches = false;
            if (touches.Count > 1 && (mainTouchA == null || mainTouchB == null || mainTouchA.getLocalPos() == Vector3.zero || mainTouchB.getLocalPos() == Vector3.zero))
                updateMainTouches = true;

            int t = 0;
            for (int i = 0; i < touchPool.Length; i++)
            {
                if (touchPool[i].getLocalPos() != Vector3.zero)
                {
                    touches[t] = touchPool[i];//these are the touches that can be queried from hypercube.input.front.touches              
                    t++;

                    averageNormalized += touchPool[i].getLocalPos();
                    averageDiff += touchPool[i].getLocalDiff();

                    if (updateMainTouches)
                    {
                        if (highX == null || touchPool[i].getLocalPos().x > highX.getLocalPos().x)
                            highX = touchPool[i];
                        if (lowX == null || touchPool[i].getLocalPos().x < lowX.getLocalPos().x)
                            lowX = touchPool[i];
                        if (highY == null || touchPool[i].getLocalPos().y > highY.getLocalPos().y)
                            highY = touchPool[i];
                        if (lowY == null || touchPool[i].getLocalPos().y < lowY.getLocalPos().y)
                            lowY = touchPool[i];
                    }
                }
            }

            if (touches.Count < 2)
            {
                pinch = 1f;
                lastTouchAngle = twist = 0f;
                mainTouchA = mainTouchB = null;
                if (touches.Count == 0)
                    averageNormalized = averageDiff = Vector3.zero;
                else //1 touch only.
                {
                    averageNormalized = touches[0].getLocalPos();
                    averageDiff = touches[0].getLocalDiff();
                }
            }
            else
            {
                averageNormalized /= (float)touches.Count;
                averageDiff /= (float)touches.Count;

                if (averageDiff.x < -.3f || averageDiff.x > .3f || averageDiff.y < -.3f || averageDiff.y > .3) //this is too fast to be a real movement, its probably an artifact.
                {
                    averageDiff = Vector3.zero;
                }

                //pinch and twist
                if (updateMainTouches)
                {
                    if ((highX.getLocalPos().x - lowX.getLocalPos().x) > (highY.getLocalPos().y - lowY.getLocalPos().y))//use the bigger of the two differences, and then use the true distance
                    {
                        mainTouchA = highX;
                        mainTouchB = lowX;
                    }
                    else
                    {
                        mainTouchA = highY;
                        mainTouchB = lowY;
                    }
                }

                float angle = HoloPlaySDK.utils.angleBetweenPoints(mainTouchA.getLocalPos(), mainTouchB.getLocalPos());
                float size = lastFrameBounds.size.x * lastFrameBounds.size.y;

                //validate everything coming out of here... ignore crazy values that may come from hardware artifacts.
                if (lastTouchAngle == 0)
                    twist = 0;
                else
                    twist =  lastTouchAngle - angle;

                if (twist < -20f || twist > 20f) //more than 20 degrees in 1 frame?!.. most likely junk. toss it.
                    twist = angle = 0f;
                lastTouchAngle = angle;

                if (lastFrameBounds.size == Vector3.zero)
                    pinch = 1f;
                else
                    pinch = size / lastSize;
                    
                if (pinch < .7f || pinch > 1.3f) //the chances that this is junk data coming from the touchscreen are very high. dump it.
                    pinch = 1f;

                lastSize = size;
            }

            lastFrameBounds = touchBounds;
            lastFrameTouchCount = touches.Count;

            processDepthTouches(touches);
            processTextureStreams();
        }



        void setHandState( bool open)
        {
            if (closedHand && open)
            {
                //hand opened
                processDepthTouchOpenClose(open);
                closedHand = false;
                if (debug)
                    Debug.Log("Hand opened.");
                return;
            }

            if (!closedHand && !open)
            {
                //hand closed
                processDepthTouchOpenClose(open);
                closedHand = true;
                if (debug)
                    Debug.Log("Hand closed.");
                return;
            }

        }

        


        private void OnDisable()
        {
            stopColorStream();
            stopDepthStream();
            shutDown();
        }

        void shutDown()
        {
            if (pluginThread != null)
            {
                pluginThread.requestStop();
                pluginThread = null;
            }

            //This reference shouldn't be null at this point anyway.
            if (thread != null)
            {
                thread.Join(); //this will cause a slight hiccup in the main calling thread, but without it can occasionally crash...
                thread = null;
            }
            //thread = null; //1 crash noted with this simpler method
        }

        //using onvalidate must be very cautious or the call can cause hte singleton in the plugin to reinitialize realsense
        //private void OnValidate()
        //{
        //    if (!hasDepthCamera())
        //        return;

        //    setTouchLerp(pluginLerp);
        //}



    }

}
