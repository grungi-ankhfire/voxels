/* VoxelVolume.cs
 *
 * A Voxel Volume is a 64x64 volume of Voxels, comprised of 64 16x16 chunks. 
 * Each volume has a single color palette, which ties into a single
 * MagickaVoxel file, usually.
 * They are a subset of a VoxelScene, which contains 8 volume for a 128x128
 * total size.
 */

using UnityEngine;
using System.Collections;

public class VoxelVolume : MonoBehaviour {

    public Palette palette;
    public Voxel[][][] voxels;
    public string filename;
    public int[] size;

	// Use this for initialization
	void Awake () {
        if (filename != null) {
            // Load the MagickaVoxel file and save palette and voxel info.
            MagickaVoxelModel model = MagickaVoxelImporter.MagickaVoxelImport(filename);
            voxels = model.voxels;
            palette = new Palette(model.palette);
            palette.colors = model.palette;
            size = model.size;
            // Instantiate the correct number of chunks
            for (int x = 0; x < Mathf.Floor(size[0]/16); x++) {
                for (int y = 0; y < Mathf.Floor(size[1]/16); y++) {
                    for (int z = 0; z < Mathf.Floor(size[2]/16); z++) {
                        GameObject new_voxel_chunk;
                        new_voxel_chunk = Instantiate(Resources.Load ("Voxel Chunk"), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
                        new_voxel_chunk.transform.parent=transform;
                        VoxelRenderer renderer = new_voxel_chunk.GetComponent<VoxelRenderer>();
                        renderer.start_x = x*renderer.chunk_x_size;
                        renderer.start_y = y*renderer.chunk_y_size;
                        renderer.start_z = z*renderer.chunk_z_size;
                    }
                }
            }


        }
	}
	
	// Update is called once per frame
	void Update () {
	}
}
