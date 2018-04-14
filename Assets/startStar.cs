using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startStar : MonoBehaviour
{

    // Use this for initialization
    public GameObject starScene;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("star"))
        {
            starScene.SetActive(true);
        }
    }
}
