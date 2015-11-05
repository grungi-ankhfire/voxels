using UnityEngine;
using System.Collections;

public class EdgeScroll : MonoBehaviour {

    public GameObject destination;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other) {
        destination.SetActive(true);
        Camera.main.GetComponent<CameraScroll>().LookAtObject(destination);
    }
}
