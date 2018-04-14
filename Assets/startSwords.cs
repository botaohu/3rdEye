using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startSwords : MonoBehaviour {

    public GameObject swordsScene; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("swords")){
            swordsScene.SetActive(true);
        
        }
    }
}

