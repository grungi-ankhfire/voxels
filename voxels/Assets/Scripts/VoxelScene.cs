using UnityEngine;
using System.Collections;

public class VoxelScene : MonoBehaviour {

    public string scene_name;
    public bool present_at_start;

	// Use this for initialization
	void Start () {
        CreateVoxelVolumes();
	}

    void CreateVoxelVolumes() {
        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 1; y++) {
                for (int z = 0; z < 2; z++) {
                    GameObject new_voxel_volume;
                    Vector3 new_position = (new Vector3(x*6.4f, y*6.4f, z*6.4f)) + transform.position;
                    new_voxel_volume = Instantiate(Resources.Load ("Voxel Volume"), new_position, Quaternion.identity) as GameObject;
                    new_voxel_volume.transform.parent = transform;
                    new_voxel_volume.name = scene_name + x.ToString() + y.ToString() + z.ToString();
                    new_voxel_volume.GetComponent<VoxelVolume>().filename = new_voxel_volume.name;
                    new_voxel_volume.GetComponent<VoxelVolume>().CreateVoxelChunks();
                }
            }
        }

    }

    public void SetVanishVoxelOffset(int fraction, Vector3 direction) {
        float distance;
        foreach (Transform child in transform) {
            if (child.gameObject.name.StartsWith(scene_name)) {
                distance = Vector3.Scale(child.localPosition, direction).magnitude;
                if (direction.x + direction.y + direction.z > 0) {
                    if (fraction <= 64 && distance < 6.4f) {
                        child.gameObject.GetComponent<VoxelVolume>().SetVanishVoxelOffset(fraction, direction);
                    } else if (fraction > 64 && distance >= 6.4f) {
                        child.gameObject.GetComponent<VoxelVolume>().SetVanishVoxelOffset(fraction-64, direction);
                    }
                } else {
                    if (fraction <= 64 && distance >= 6.4f) {
                        child.gameObject.GetComponent<VoxelVolume>().SetVanishVoxelOffset(fraction, direction);
                    } else if (fraction > 64 && distance < 6.4f) {
                        child.gameObject.GetComponent<VoxelVolume>().SetVanishVoxelOffset(fraction-64, direction);
                    }
                }
            }
        } 
    }

    public void SetAppearVoxelOffset(int fraction, Vector3 direction) {
        float distance;
        foreach (Transform child in transform) {
            if (child.gameObject.name.StartsWith(scene_name)) {
                distance = Vector3.Scale(child.localPosition, direction).magnitude;
                if (direction.x + direction.y + direction.z > 0) {
                    if (fraction <= 64 && distance < 6.4f) {
                        child.gameObject.GetComponent<VoxelVolume>().SetAppearVoxelOffset(fraction, direction);
                    } else if (fraction > 64 && distance >= 6.4f) {
                        child.gameObject.GetComponent<VoxelVolume>().SetAppearVoxelOffset(fraction-64, direction);
                    }
                } else {
                    if (fraction <= 64 && distance >= 6.4f) {
                        child.gameObject.GetComponent<VoxelVolume>().SetAppearVoxelOffset(fraction, direction);
                    } else if (fraction > 64 && distance < 6.4f) {
                        child.gameObject.GetComponent<VoxelVolume>().SetAppearVoxelOffset(fraction-64, direction);
                    }
                }
            }
        } 
    }

}
