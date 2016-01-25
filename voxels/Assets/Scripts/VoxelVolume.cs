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

	}
	
    public void CreateVoxelChunks() {
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
                        if (model.chunks[x][y][z]) {
                            GameObject new_voxel_chunk;
                            new_voxel_chunk = Instantiate(Resources.Load ("Voxel Chunk"), transform.position + new Vector3(x*1.6f, y*1.6f, z*1.6f), Quaternion.identity) as GameObject;
                            new_voxel_chunk.transform.parent=transform;
                            new_voxel_chunk.name = "VoxelChunk" + x.ToString() + y.ToString() + z.ToString();
                            VoxelRenderer renderer = new_voxel_chunk.GetComponent<VoxelRenderer>();
                            renderer.start_x = x*renderer.chunk_x_size;
                            renderer.start_y = y*renderer.chunk_y_size;
                            renderer.start_z = z*renderer.chunk_z_size;
                            if (transform.parent.gameObject.GetComponent<VoxelScene>().present_at_start) {
                                renderer.offset_end_x = 0;
                                renderer.dirty = true;
                            }
                        }
                    }
                }
            }


        }
    }

    public void SetVanishVoxelOffset(int fraction, Vector3 direction) {
        float distance;
        foreach (Transform child in transform) {
                child.gameObject.GetComponent<VoxelRenderer>().SetVanishVoxelOffset(fraction, direction);
        } 
    }

    public void SetAppearVoxelOffset(int fraction, Vector3 direction) {
        float distance;
        foreach (Transform child in transform) {
            child.gameObject.GetComponent<VoxelRenderer>().SetAppearVoxelOffset(fraction, direction);
        } 
    }



}
