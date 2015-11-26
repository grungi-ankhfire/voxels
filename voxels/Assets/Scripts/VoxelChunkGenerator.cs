using UnityEngine;
using System.Collections;

public class VoxelChunkGenerator : MonoBehaviour {

    public VoxelRenderer voxel_chunk;

	// Use this for initialization
	void Start () {
        for (int x = 0; x < 1; x++) {
            for (int y = 0; y < 1; y++) {
                for (int z = 0; z < 1; z++) {
                    VoxelRenderer new_voxel_chunk;
                    new_voxel_chunk = (VoxelRenderer) Instantiate(voxel_chunk, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    new_voxel_chunk.start_x = x*new_voxel_chunk.chunk_x_size;
                    new_voxel_chunk.start_y = y*new_voxel_chunk.chunk_y_size;
                    new_voxel_chunk.start_z = z*new_voxel_chunk.chunk_z_size;
                    
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
