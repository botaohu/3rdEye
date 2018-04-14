using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class noScript : MonoBehaviour {

    public GameObject transition;
    public AudioSource sceneSwitch;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("No")) {
            StartCoroutine(Reset());

        }
    }

    IEnumerator Reset() {
        sceneSwitch.Play();
        transition.SetActive(true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Start");

    }
}
