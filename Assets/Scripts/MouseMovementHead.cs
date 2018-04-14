using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloPlaySDK;

public class MouseMovementHead : MonoBehaviour {
	HoloPlaySDK_UI.CalibratedPointer pointerScript; 

	Rigidbody rb;

	[SerializeField] Camera testCam;

	float zOffset = 0;

	void Awake(){
		pointerScript = GetComponent<HoloPlaySDK_UI.CalibratedPointer> ();
		rb = GetComponent<Rigidbody> ();
	}

	// Use this for initialization
	void OnEnable () {
		pointerScript.enabled = false;
	}

	void OnDisable(){
		pointerScript.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		float z = HoloPlay.Main.transform.position.z;

		Vector3 mousePos = testCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z + zOffset - testCam.transform.position.z));

		rb.position = mousePos;

		if (Input.GetMouseButton (0)) {
			zOffset += 10f * Time.deltaTime;
		} else if (Input.GetMouseButton (1)) {
			zOffset -= 10f * Time.deltaTime;
		}

	}
}
