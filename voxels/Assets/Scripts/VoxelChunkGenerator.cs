using UnityEngine;
using System.Collections;

public class VoxelChunkGenerator : MonoBehaviour {

    public VoxelRenderer voxel_chunk;

	// Use this for initialization
	void Start () {
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                for (int z = 0; z < 8; z++) {
                    VoxelRenderer new_voxel_chunk;
                    new_voxel_chunk = (VoxelRenderer) Instantiate(voxel_chunk, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                    new_voxel_chunk.start_x = x*new_voxel_chunk.chunk_x_size;
                    new_voxel_chunk.start_y = y*new_voxel_chunk.chunk_y_size;
                    new_voxel_chunk.start_z = z*new_voxel_chunk.chunk_z_size;
                    if (y == 7) {
                        new_voxel_chunk.wavy_y = true;
                        new_voxel_chunk.delay = Mathf.Max(x,z) * 1;
                    }
                    
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
