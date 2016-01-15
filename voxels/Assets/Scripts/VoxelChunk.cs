using UnityEngine;
using System.Collections;

public class VoxelChunk : MonoBehaviour {


    public int[,,] voxels;
    public int x_size = 16;
    public int y_size = 16;
    public int z_size = 16;

    public int density = 50;

    void Awake() {
        voxels = new int[x_size, y_size, z_size];
    }


}
