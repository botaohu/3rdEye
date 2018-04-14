using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnOnGravity : MonoBehaviour {


    private Rigidbody rigidbody; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("hand")) {
            rigidbody.useGravity = false; 

        }
    }
}
