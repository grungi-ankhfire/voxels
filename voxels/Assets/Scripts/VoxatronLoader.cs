using UnityEngine;
using System.Collections;
using System.IO;

public class VoxatronLoader : MonoBehaviour {

    public GameObject voxel;
    public string file_to_load;
    public int x_size, y_size, z_size;
    public int[,] palette = new int[256, 3];
    public int[,,] voxels;

    bool drew = false;
    bool loaded = false;

	// Use this for initialization
	void Start () {
        LoadLevel();
	}
	
    void LoadLevel() {

        int line_index = 0;    
        StreamReader sr = new StreamReader(Application.dataPath + "/" + file_to_load);
        string line;
        string[] tokens;

        int x = 0, y = 0, z = 0;
        int current_voxel;

        while ((line = sr.ReadLine()) != null) {
            line_index++;
            if (line_index == 3) {
                tokens = line.Split(' ');
                int.TryParse(tokens[0], out x_size);
                int.TryParse(tokens[1], out y_size);
                int.TryParse(tokens[2], out z_size);
                voxels = new int[x_size, y_size, z_size];
            }
            if (line_index >= 5 && line_index <= 260) {
                tokens = line.Split(' ');
                int.TryParse(tokens[0], out palette[line_index-5, 0]);
                int.TryParse(tokens[1], out palette[line_index-5, 1]);
                int.TryParse(tokens[2], out palette[line_index-5, 2]);                
            }
            if (line_index >= 262) {
                tokens = line.Split(' ');
                foreach (string token in tokens) {
                    if (int.TryParse(token, out current_voxel)) {
                        voxels[x,y,z] = current_voxel;
/*                        GameObject new_voxel;
                        if (current_voxel != 0) {
                            new_voxel = (GameObject) Instantiate(voxel, new Vector3(x, y, z), Quaternion.identity);
                            new_voxel.renderer.material.color = new Color(palette[current_voxel,0]/256f, palette[current_voxel,1]/256f, palette[current_voxel,2]/256f);
                        }*/
                        x+=1;
                        if (x == x_size){
                            x = 0;
                            y += 1;
                            if (y == y_size) {
                                y = 0;
                                z += 1;
                            }
                        }
                    }
                }
            }
        }
        loaded = true;
    }

    bool isVoxelVisible(int x, int y, int z) {

        if (x == 0 || x == x_size-1 || y == 0 || y == y_size-1 || z == 0 || z == z_size-1) {
            return true;
        } else if (voxels[x-1, y, z] == 0
                   || voxels[x+1, y, z] == 0
                   || voxels[x, y-1, z] == 0
                   || voxels[x, y+1, z] == 0
                   || voxels[x, y, z-1] == 0
                   || voxels[x, y, z+1] == 0) {
            return true;
        }
        return false;
    }

	// Update is called once per frame
	void Update () {
	
        if (drew || !loaded) {
            return;
        }

	    // Draw the voxel buffer
        for (int x = 0; x < x_size; x++) {
            for (int y = 0; y < y_size; y++) {
                for (int z = 0; z < z_size; z++) {
                    if (voxels[x,y,z] != 0 && isVoxelVisible(x,y,z)) {
                        GameObject new_voxel;
                        new_voxel = (GameObject) Instantiate(voxel, new Vector3(x, y, z), Quaternion.identity);
                        new_voxel.GetComponent<Renderer>().material.color = new Color(palette[voxels[x,y,z],0]/256f, palette[voxels[x,y,z],1]/256f, palette[voxels[x,y,z],2]/256f);
                    }
                }
            }
        }

        drew = true;
    }
}
