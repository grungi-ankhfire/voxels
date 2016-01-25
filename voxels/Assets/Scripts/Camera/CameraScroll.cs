using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;

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

    public GameObject character;
    private Vector3 character_old_pos;
    private Vector3 character_new_pos;

    private float previous_fracJourney;

    private Vector3 movementDirection;

    // DEBUG STUFF
    private int voxel_changes;

	// Use this for initialization
	void Start () {
	   offset = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (isMoving) {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            float rounded_fracJourney = Mathf.Floor(fracJourney*128f)/128f;
            character.transform.position = Vector3.Lerp(character_old_pos, character_new_pos, fracJourney);        	
            if (rounded_fracJourney > previous_fracJourney) {   
                // DEBUG STUFF
                voxel_changes++;
                transform.position = Vector3.Lerp(startPosition, endPosition, rounded_fracJourney);
                new_focus.GetComponent<VoxelScene>().SetAppearVoxelOffset((int) Mathf.Floor(rounded_fracJourney*128.0f), movementDirection);
                old_focus.GetComponent<VoxelScene>().SetVanishVoxelOffset((int) Mathf.Floor(rounded_fracJourney*128.0f), movementDirection);
                previous_fracJourney = rounded_fracJourney;
            }
            if (transform.position == endPosition) {
                isMoving = false;
                foreach (Transform child in new_focus.transform)
                     {
                             child.gameObject.SetActive(true);
                     } 
                old_focus.SetActive(false);
                old_focus = new_focus;
                character.GetComponent<ThirdPersonUserControl>().enabled = true;
                // DEBUG STUFF
                print("Voxel changes:" + voxel_changes);
            }
        }
    }

    public void LookAtObject(GameObject obj) {
        new_focus = obj;
        foreach (Transform child in old_focus.transform)
            {
                if (!child.gameObject.name.StartsWith(child.transform.parent.gameObject.GetComponent<VoxelScene>().scene_name)) {
                    child.gameObject.SetActive(false);
                }
            } 
        startPosition = Camera.main.transform.position;
        endPosition = obj.transform.position + offset;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, endPosition);
        character.GetComponent<ThirdPersonUserControl>().enabled = false;
        character_old_pos = character.transform.position;
        movementDirection = (endPosition-startPosition);
        movementDirection.Normalize();
        character_new_pos = character_old_pos + Vector3.Scale(movementDirection, character.GetComponentsInChildren<Collider>()[0].bounds.size);
        isMoving = true;
        previous_fracJourney = 0f;
        // DEBUG STUFF
        voxel_changes = 0;
    } 

}
