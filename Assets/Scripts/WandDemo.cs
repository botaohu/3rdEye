using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandDemo : MonoBehaviour {

    WandManager wm;
    public int haptic_num;
    public float yaw_zero;
    bool overlap;
    GameObject pickup;
    
	// Use this for initialization
	void Start () {
        wm = WandManager.Instance;
        haptic_num = 0;
	}

    public void enter(GameObject t)
    {
        pickup = t;
        overlap = true;
        wm.sendHapticEffect(5);
    }

    public void exit()
    {
        overlap = false;
     //   wm.sendHapticEffect(10);
    }
	
	// Update is called once per frame
	void Update () {

        gameObject.transform.localRotation = wm.GetQuat();
        gameObject.transform.Rotate(new Vector3(0, -yaw_zero, 0), Space.World);
        if (Input.GetKeyDown(KeyCode.Space))
        {
           yaw_zero = gameObject.transform.eulerAngles.y;
        }
        if (wm.getButton(WandManager.Button.LEFT))
        {
            transform.Find("ball2").localPosition = new Vector3(0,0,.5f);
            if (overlap)
            {
                pickup.transform.position = transform.Find("ball1").position;
            }
        }
        else
        {
            transform.Find("ball2").localPosition = new Vector3(0, 0, 0);
        }
 
    }
}
