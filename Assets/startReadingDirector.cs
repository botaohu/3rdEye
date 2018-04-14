using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class startReadingDirector : MonoBehaviour {

    public GameObject transition;
    public bool waitTime;
    public AudioSource startSound;
    public bool hitAgain; 

	// Use this for initialization
	void Start () {
        waitTime = true;
        StartCoroutine(WaitTime());
        hitAgain = true; 
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Player")) {
            Debug.Log("you are in the trigger");
            hitAgain = false; 
            StartCoroutine(Shop());
            


        }
    }

    IEnumerator WaitTime() {
        yield return new WaitForSeconds(3f);
        waitTime = false; 
    }

    IEnumerator Shop() {

        if (hitAgain == false)
        {
            transition.SetActive(true);
            startSound.Play();
            yield return new WaitForSeconds(2f);

            Debug.Log("i'm about to start the scene");
            SceneManager.LoadScene("Shop");
        }
    }
}
