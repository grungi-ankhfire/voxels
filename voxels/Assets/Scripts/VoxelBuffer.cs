using UnityEngine;
using System.Collections;

public class VoxelBuffer : MonoBehaviour {

    private static VoxelBuffer _instance;
 
    public static VoxelBuffer instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<VoxelBuffer>();
 
                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
 
    void Awake() 
    {
        if(_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            voxel_buffer = new Voxel[buffer_x_size][][];
            for (int x = 0; x < buffer_x_size; x++) {
                voxel_buffer[x] = new Voxel[buffer_y_size][];
                for (int y = 0; y < buffer_y_size; y++) {
                    voxel_buffer[x][y] = new Voxel[buffer_z_size];
                }
            }
            FillBufferFromMagickaVoxel();
            //FillBufferRandomly();
            //FillBufferRandomlyWithDensity();
            //FillBufferBlock();
            DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if(this != _instance)
                Destroy(this.gameObject);
        }
    }

    public Voxel[][][] voxel_buffer;
    public int buffer_x_size = 32;
    public int buffer_y_size = 32;
    public int buffer_z_size = 32;
    public int density = 50;
    public int voxel_types = 1;

    public string vox_file;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FillBufferFromMagickaVoxel() {
        voxel_buffer = MagickaVoxelImporter.MagickaVoxelImport(vox_file).voxels;
    }

    void FillBufferRandomly() {
        for (int x = 0; x < buffer_x_size; x++) {
            for (int y = 0; y < buffer_y_size; y++) {
                for (int z = 0; z < buffer_z_size; z++) {
                    //voxel_buffer[x][y][z] = Random.Range(0, 2);
                }
            }
        }
    }

    void FillBufferRandomlyWithDensity() {
        for (int x = 0; x < buffer_x_size; x++) {
            for (int y = 0; y < buffer_y_size; y++) {
                for (int z = 0; z < buffer_z_size; z++) {
                    int vox_prob = Random.Range(0,100);
                    int vox = 0;
                    if (vox_prob < density) {
                        vox = Random.Range(1, voxel_types+1);
                        voxel_buffer[x][y][z] = new Voxel(0);
                    } else {
                        voxel_buffer[x][y][z] = null;
                    }
                }
            }
        }
    }

    void FillBufferBlock() {
        for (int x = 0; x < buffer_x_size; x++) {
            for (int y = 0; y < buffer_y_size; y++) {
                for (int z = 0; z < buffer_z_size; z++) {
                    int voxel = 0;
                    if (x < 64 && y < 64 && z < 64) voxel = 1;
                    //voxel_buffer[x][y][z] = voxel;
                }
            }
        }
    }
}
