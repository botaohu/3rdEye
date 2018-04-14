using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloPlaySDK;

public class grabTest : depthTouchTarget
{

    public GameObject[] fingers;
    public Collider grabbable;
    public float grabRange = .05f;
   
    public Transform scaleTest;
    public Transform centerTest;

    public depthPlugin d;




    // Update is called once per frame
    void Update ()
    {
        scaleTest.localScale *= d.pinch;
        scaleTest.Rotate(0f, d.twist, 0f);
        centerTest.localPosition = d.averageNormalized;

        centerTest.Translate(.1f, 0f, 0f, Space.World);

        if (scaleTest.localScale.x < .001f)
            scaleTest.localScale = new Vector3(.001f, .001f, .001f);

        //grabbing
        if (grabbable.bounds.Contains(d.averageNormalized) && 
            (d.closedHand || d.isHolding(grabbable.transform.position, grabRange)))
        {
            grabbable.transform.position = d.averageNormalized;
            grabbable.transform.Rotate(0f, 0f, d.twist);
        }
    }

    void resetAll()
    {
        foreach (GameObject g in fingers)
            g.transform.position = Vector3.zero;
    }


    //Depth touches.  These are touches detected in the air from a depth camera like the Intel RealSense
    public override void onDepthTouch(List<depthTouch> touches)
    {
        resetAll();
        foreach (depthTouch d in touches)
        {
            fingers[d.id].transform.localPosition = d.getLocalPos();
        }
    }

    public override void onNoDepthTouches()
    {
        resetAll();
    }

    //sends a message if it is open or closed
    public override void onHandState(bool open)
    {

    }
}
