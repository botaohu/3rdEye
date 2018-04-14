using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

public class noMultithreadTest : MonoBehaviour {

	/// These may crash or cause undefined behavior if the texture providedis not the proper format.
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

	[DllImport("depthPlugin")]
	public static extern void update();//try to call this at least 60fps

	[DllImport("depthPlugin")]
	public static extern void shutDown();//destroy the realsense class inside the plugin (it will be recreated if you use the other methods again)

	[DllImport("depthPlugin")]
	static extern bool prepareTouchCollection();//call this first, returns false if the plugin is not properly loaded
	[DllImport("depthPlugin")]
	public static extern float getTouchData(); //then collect a float array piecemeal. 
	[DllImport("depthPlugin")]
	public static extern void setCalibration(float meterDiffNear, float zAverageNear, float meterDiffFar, float zAverageFar);


	//debug messages
	private delegate void debugCallback(string message);
	[DllImport("depthPlugin")]
	private static extern void registerDebugCallback(debugCallback callback);

	private static void debugMethod(string message)
	{
		if (message.Contains("error"))
			shutDown();

		Debug.Log("HoloPlaySDK Depth: " + message);
	}


	void Start()
	{
		registerDebugCallback(new debugCallback(debugMethod)); //allow plugin to print to console
		setCalibration(4157.5f, 908.75f, 2435f, 2556.5f);
		Debug.Log ("started");
	}

	void Update()
	{
		update ();
		processTextureStreams ();
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



}
