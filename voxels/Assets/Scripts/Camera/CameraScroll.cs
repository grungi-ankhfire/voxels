using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour {

    Vector3 offset = new Vector3(-4.5f, 5.5f, -4.5f);

    Vector3 startPosition;
    Vector3 endPosition;

    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;

    private bool isMoving = false;
    public GameObject old_focus;
    private GameObject new_focus;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isMoving) {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);        	
	        if (transform.position == endPosition) {
                isMoving = false;
                foreach (Transform child in new_focus.transform)
                     {
                             child.gameObject.SetActive(true);
                     } 
                old_focus.SetActive(false);
                old_focus = new_focus;
            }
        }
    }

    public void LookAtObject(GameObject obj) {
        new_focus = obj;
        foreach (Transform child in old_focus.transform)
            {
                child.gameObject.SetActive(false);
            } 
        startPosition = Camera.main.transform.position;
        endPosition = obj.transform.position + offset;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, endPosition);
        isMoving = true;
    } 

}
