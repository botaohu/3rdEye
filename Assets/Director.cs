using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Director : MonoBehaviour
{

    public bool go;
    private bool tower;
    private bool star; 


    //scenes for instantiation
    public GameObject heartOfSwordScene;
    public GameObject towerScene;
    public GameObject starScene;

    private Rigidbody swordFreeze;
    private Rigidbody towerFreeze;
    private Rigidbody starFreeze;
    private Collider swordBox;
    private Collider towerBox;
    private Collider starBox; 

    //audiosources
    public AudioSource welcome;
    public AudioSource interesting;
    public AudioSource threeCard;
    public AudioSource painfulPast;
    public AudioSource presentRealiz;
    public AudioSource brightFuture;
    public AudioSource pastExplanation;
    public AudioSource presentExplanation;
    public AudioSource futureExplanation;
    public AudioSource thankyou;
    public AudioSource cmon;
    public AudioSource waitingfor;
    public AudioSource theyouth;
    public AudioSource usefingers; 

    //animations
    public Animator psychicHands;
    public Animator psychicLips;
    public Animator swordsAnim;
    public Animator towerAnim;
    public Animator starAnim;
    public Animator psychicBody;

    //cards
    public GameObject swordsCard;
    public GameObject towerCard;
    public GameObject starCard;
    public GameObject payment;

    public GameObject heartCollider;
    public Transform swordSpace;
    public GameObject towerCollider;
    public Transform towerSpace;
    public GameObject starCollider;
    public Transform starSpace;


    // Use this for initialization
    void Start()
    {
        go = false;
        tower = false;
        star = false; 
        StartCoroutine(Begin());



    }
    // Update is called once per frame
    void Update() {


        if (Input.GetKeyDown("space") && (go == true))
        {

            StartCoroutine(PullCards());

        }

        else {
            return;
        }



    }

    IEnumerator Begin()
    {
        yield return new WaitForSeconds(5f);

        psychicLips.SetLayerWeight(0, 0);
        psychicLips.SetLayerWeight(1, 1);
        welcome.Play();
        yield return new WaitForSeconds(12f);

        psychicLips.SetLayerWeight(0, 1);
        psychicLips.SetLayerWeight(1, 0);
        go = true;

    }

    IEnumerator PullCards()
    {
        go = false;
        psychicLips.SetLayerWeight(0, 0);
        psychicLips.SetLayerWeight(1, 1);
        interesting.Play();
        yield return new WaitForSeconds(2f);
        psychicHands.SetLayerWeight(0, 0);
        psychicHands.SetLayerWeight(1, 1);
        psychicBody.SetLayerWeight(0, 0);
        psychicBody.SetLayerWeight(1, 1);
        yield return new WaitForSeconds(5f);
        psychicLips.SetLayerWeight(0, 1);
        psychicLips.SetLayerWeight(1, 0);
        psychicHands.SetLayerWeight(0, 1);
        psychicHands.SetLayerWeight(1, 0);
        psychicBody.SetLayerWeight(0, 1);
        psychicBody.SetLayerWeight(1, 0);
        psychicLips.SetLayerWeight(0, 0);
        psychicLips.SetLayerWeight(1, 1);
        threeCard.Play();
        yield return new WaitForSeconds(2f);
        psychicLips.SetLayerWeight(0, 1);
        psychicLips.SetLayerWeight(1, 0);
        yield return new WaitForSeconds(3f);
        swordsCard.SetActive(true);
        swordsCard.transform.eulerAngles = new Vector3(0, 0, 0);
        swordsAnim.SetLayerWeight(0, 0);
        swordsAnim.SetLayerWeight(1, 1);
        psychicLips.SetLayerWeight(0, 1);
        psychicLips.SetLayerWeight(1, 0);
        yield return new WaitForSeconds(3f);
        waitingfor.Play();
        yield return new WaitForSeconds(8f);
        pastExplanation.Play();
        psychicLips.SetLayerWeight(0, 0);
        psychicLips.SetLayerWeight(1, 1);
    
        yield return new WaitForSeconds(10f);

        if (heartOfSwordScene.activeSelf)
        {
            StartCoroutine(waitForTower());
        }
        else
        {
            yield return new WaitForSeconds(5f);
            heartOfSwordScene.SetActive(true);
            StartCoroutine(waitForTower());
        }
       /* painfulPast.Play();
        yield return new WaitForSeconds(10f);
        heartOfSwordScene.SetActive(false);
        heartCollider.SetActive(false);
       
        swordFreeze = swordsCard.GetComponent<Rigidbody>();
        swordFreeze.isKinematic = true;
        swordBox = swordsCard.GetComponent<Collider>();
        swordBox.enabled = false;
        swordsCard.transform.parent = swordSpace.transform;
        swordsCard.transform.localPosition = Vector3.zero;
        swordsCard.transform.Rotate(90, 0, 0);
        
        psychicLips.SetLayerWeight(0, 1);
        psychicLips.SetLayerWeight(1, 0);


        psychicHands.SetLayerWeight(0, 0);
        psychicHands.SetLayerWeight(1, 1);
        psychicBody.SetLayerWeight(0, 0);
        psychicBody.SetLayerWeight(1, 1);
        yield return new WaitForSeconds(5f);
        psychicHands.SetLayerWeight(0, 1);
        psychicHands.SetLayerWeight(1, 0);
        psychicBody.SetLayerWeight(0, 1);
        psychicBody.SetLayerWeight(1, 0);
        towerCard.SetActive(true);
        tower = true;
        yield return new WaitForSeconds(4f);
        psychicLips.SetLayerWeight(0, 0);
        psychicLips.SetLayerWeight(1, 1);
        presentExplanation.Play();
        psychicLips.SetLayerWeight(0, 1);
        psychicLips.SetLayerWeight(1, 0);
        yield return new WaitForSeconds(9f);
        
        presentRealiz.Play();
        yield return new WaitForSeconds(10f);
        towerScene.SetActive(false);
        towerCollider.SetActive(false);
       
        towerFreeze = towerCard.GetComponent<Rigidbody>();
        towerFreeze.isKinematic = true;
        towerCard.transform.parent = towerSpace.transform;
        towerCard.transform.localPosition = Vector3.zero;
        towerCard.transform.Rotate(90, 0, 0);
        towerBox = towerCard.GetComponent<Collider>();
        towerBox.enabled = false; 

        psychicHands.SetLayerWeight(0, 0);
        psychicHands.SetLayerWeight(1, 1);
        psychicBody.SetLayerWeight(0, 0);
        psychicBody.SetLayerWeight(1, 1);
        yield return new WaitForSeconds(5f);
        psychicHands.SetLayerWeight(0, 1);
        psychicHands.SetLayerWeight(1, 0);
        psychicBody.SetLayerWeight(0, 1);
        psychicBody.SetLayerWeight(1, 0);
        starCard.SetActive(true);
        yield return new WaitForSeconds(4f);
        psychicLips.SetLayerWeight(0, 0);
        psychicLips.SetLayerWeight(1, 1);
        futureExplanation.Play();
        psychicLips.SetLayerWeight(0, 1);
        psychicLips.SetLayerWeight(1, 0);
        yield return new WaitForSeconds(10f);
        */
       




    }

    IEnumerator waitForTower() {

            painfulPast.Play();
            yield return new WaitForSeconds(15f);
            heartOfSwordScene.SetActive(false);
            heartCollider.SetActive(false);

            swordFreeze = swordsCard.GetComponent<Rigidbody>();
            swordFreeze.isKinematic = true;
            swordBox = swordsCard.GetComponent<Collider>();
            swordBox.enabled = false;
            swordsCard.transform.parent = swordSpace.transform;
            swordsCard.transform.localPosition = Vector3.zero;
            swordsCard.transform.Rotate(90, 0, 0);

            psychicLips.SetLayerWeight(0, 1);
            psychicLips.SetLayerWeight(1, 0);


            psychicHands.SetLayerWeight(0, 0);
            psychicHands.SetLayerWeight(1, 1);
            psychicBody.SetLayerWeight(0, 0);
            psychicBody.SetLayerWeight(1, 1);
            yield return new WaitForSeconds(5f);
            psychicHands.SetLayerWeight(0, 1);
            psychicHands.SetLayerWeight(1, 0);
            psychicBody.SetLayerWeight(0, 1);
            psychicBody.SetLayerWeight(1, 0);
            towerCard.SetActive(true);
            tower = true;
            yield return new WaitForSeconds(4f);
            psychicLips.SetLayerWeight(0, 0);
            psychicLips.SetLayerWeight(1, 1);
            presentExplanation.Play();
        yield return new WaitForSeconds(7f);
        cmon.Play(); 
        yield return new WaitForSeconds(10f);
        if (towerScene.activeSelf)
        {

            StartCoroutine(waitForStar());
        }
        else {

            yield return new WaitForSeconds(5f);
            towerScene.SetActive(true);
            StartCoroutine(waitForStar());
        }




    }

    IEnumerator waitForStar() {

        presentRealiz.Play();
        yield return new WaitForSeconds(15f);
        towerScene.SetActive(false);
        towerCollider.SetActive(false);

        towerFreeze = towerCard.GetComponent<Rigidbody>();
        towerFreeze.isKinematic = true;
        towerCard.transform.parent = towerSpace.transform;
        towerCard.transform.localPosition = Vector3.zero;
        towerCard.transform.Rotate(90, 0, 0);
        towerBox = towerCard.GetComponent<Collider>();
        towerBox.enabled = false;

        psychicHands.SetLayerWeight(0, 0);
        psychicHands.SetLayerWeight(1, 1);
        psychicBody.SetLayerWeight(0, 0);
        psychicBody.SetLayerWeight(1, 1);
        yield return new WaitForSeconds(5f);
        psychicHands.SetLayerWeight(0, 1);
        psychicHands.SetLayerWeight(1, 0);
        psychicBody.SetLayerWeight(0, 1);
        psychicBody.SetLayerWeight(1, 0);
        starCard.SetActive(true);
        yield return new WaitForSeconds(4f);
        psychicLips.SetLayerWeight(0, 0);
        psychicLips.SetLayerWeight(1, 1);
        futureExplanation.Play();
        yield return new WaitForSeconds(4f);
        psychicLips.SetLayerWeight(0, 1);
        psychicLips.SetLayerWeight(1, 0);

        if (starScene.activeSelf) {

            StartCoroutine(waitForPayment());
        }
        else
        {

            yield return new WaitForSeconds(5f);
            starScene.SetActive(true);
            StartCoroutine(waitForPayment());
        }
    }

    IEnumerator waitForPayment() {
      
        yield return new WaitForSeconds(15f);
        psychicLips.SetLayerWeight(0, 0);
        psychicLips.SetLayerWeight(1, 1);
        brightFuture.Play();
        yield return new WaitForSeconds(4f);
        psychicLips.SetLayerWeight(0, 1);
        psychicLips.SetLayerWeight(1, 0);
        starScene.SetActive(false);
        starCollider.SetActive(false);
        starFreeze = starCard.GetComponent<Rigidbody>();
        starFreeze.isKinematic = true;
        starCard.transform.parent = starSpace.transform;
        starCard.transform.localPosition = Vector3.zero;
        starBox = starCard.GetComponent<Collider>();
        starBox.enabled = false;

        starCard.transform.Rotate(90, 0, 0);
        yield return new WaitForSeconds(3f);
        psychicLips.SetLayerWeight(0, 0);
        psychicLips.SetLayerWeight(1, 1);
        thankyou.Play();
        payment.SetActive(true);
        yield return new WaitForSeconds(6f);
        psychicLips.SetLayerWeight(0, 1);
        psychicLips.SetLayerWeight(1, 0);
        yield return new WaitForSeconds(20f);
        SceneManager.LoadScene("End");

    }
}

