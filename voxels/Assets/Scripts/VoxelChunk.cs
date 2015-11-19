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
        FillChunkRandomlyWithDensity();
    }

    void FillChunkRandomlyWithDensity() {
        for (int x = 0; x < x_size; x++) {
            for (int y = 0; y < y_size; y++) {
                for (int z = 0; z < z_size; z++) {
                    int vox_prob = Random.Range(0,100);
                    int vox = 0;
                    if (vox_prob < density) {
                        vox = 1;
                    }
                    voxels[x, y, z] = vox;
                }
            }
        }
    }


}
