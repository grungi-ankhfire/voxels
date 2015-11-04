using UnityEngine;
using System.Collections;

public class ChangeFocus : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseUp() {
        Camera.main.GetComponent<CameraScroll>().LookAtObject(gameObject);
    }
}
