using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startTower : MonoBehaviour {

    // Use this for initialization
    public GameObject towerScene; 
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("tower"))
        {
            towerScene.SetActive(true);
        }
    }
}
