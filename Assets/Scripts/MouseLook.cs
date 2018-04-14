using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

	[SerializeField] GameObject cursor;
	[SerializeField] float sensitivity = 200f;
	[SerializeField] Camera cam;
	[SerializeField] Vector3 minCameraRotation;
	[SerializeField] Vector3 maxCameraRotation;

	Vector3 newLookRotation;
	Vector3 cursorScreenPos;

	float cursorX;
	float cursorY;
    float angleY  = 0.0f;


    void Update () {

		cursorScreenPos = cam.WorldToScreenPoint (cursor.transform.localPosition);
//		cursorX = cursorScreenPos.x;
//		cursorY = cursorScreenPos.y;

		cursorX = cursor.transform.localPosition.x;
		cursorY = cursor.transform.localPosition.y;

		transform.Rotate (-cursorY * Time.deltaTime * sensitivity, cursorX * Time.deltaTime * sensitivity, 0f);
		transform.eulerAngles = new Vector3(ClampAngle(transform.eulerAngles.x, minCameraRotation.x, maxCameraRotation.x), ClampAngle(transform.eulerAngles.y, minCameraRotation.y, maxCameraRotation.y), 0.0f);

		/*newLookRotation = new Vector3 (Mathf.Clamp(transform.eulerAngles.x, minCameraRotation.x, maxCameraRotation.x), Mathf.Clamp(transform.eulerAngles.y, minCameraRotation.y, maxCameraRotation.y), 0f);
		transform.eulerAngles = newLookRotation;*/
		//Debug.Log("X :" + cursorX + " Y :" + cursorY);
	}

	public static float ClampAngle(float angle, float min, float max)
 {
     angle = Mathf.Repeat(angle, 360);
     min = Mathf.Repeat(min, 360);
     max = Mathf.Repeat(max, 360);
     bool inverse = false;
     var tmin = min;
     var tangle = angle;
     if(min > 180)
     {
         inverse = !inverse;
         tmin -= 180;
     }
     if(angle > 180)
     {
         inverse = !inverse;
         tangle -= 180;
     }
     var result = !inverse ? tangle > tmin : tangle < tmin;
     if(!result)
         angle = min;

     inverse = false;
     tangle = angle;
     var tmax = max;
     if(angle > 180)
     {
         inverse = !inverse;
         tangle -= 180;
     }
     if(max > 180)
     {
         inverse = !inverse;
         tmax -= 180;
     }
 
     result = !inverse ? tangle < tmax : tangle > tmax;
     if(!result)
         angle = max;
     return angle;
 }
}
