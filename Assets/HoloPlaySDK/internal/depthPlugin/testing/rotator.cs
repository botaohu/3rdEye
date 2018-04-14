using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour {

    public float speed = .1f;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(speed, speed / 2f, speed / 3f);
	}
}
