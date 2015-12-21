using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelRenderer : MonoBehaviour {

    private int[][][] voxel_buffer;
    public int chunk_x_size = 16;
    public int chunk_y_size = 16;
    public int chunk_z_size = 16;

    public int start_x = 0;
    public int start_y = 0;
    public int start_z = 0;

    public int offset_start_x = 0;
    public int offset_start_y = 0;
    public int offset_start_z = 0;
    public int offset_end_x = 0;
    public int offset_end_y = 0;
    public int offset_end_z = 0;

    private int[] dims;
    private int[] buffer_size;
    private int[] starting;
    private int[] offset_start;
    private int[] offset_end;

    // Testing stuff :p
    public bool wavy_y = false;
    public int delay = 0;
    public int frame = 0;
    private int wavy_direction = 1;
    // End testing stuff


    public List<Vector3> newVertices = new List<Vector3>();
    public List<Color> newColors = new List<Color>();
    public List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();
  
    private float tUnit = 0.25f;
      
    private Mesh mesh;
    private MeshCollider col;
  
    private int faceCount;

    private int[][] mask;

    public bool dirty = false;

	// Use this for initialization
	void Start () {
        voxel_buffer = VoxelBuffer.instance.voxel_buffer;

        dims = new int[3] {chunk_x_size, chunk_y_size, chunk_z_size};
        buffer_size = new int[3] {VoxelBuffer.instance.buffer_x_size,
                                  VoxelBuffer.instance.buffer_y_size,
                                  VoxelBuffer.instance.buffer_z_size};
        starting = new int[3] {start_x, start_y, start_z};
        offset_start = new int[3] {offset_start_x, offset_start_y, offset_start_z};
        offset_end = new int[3] {offset_end_x, offset_end_y, offset_end_z};

        mesh = GetComponent<MeshFilter> ().mesh;
        mesh.MarkDynamic();
        col = GetComponent<MeshCollider> ();

        DrawVoxelBufferChunk();
	}

	// Update is called once per frame
	void Update () {
        if (dirty) {
            offset_start = new int[3] {offset_start_x, offset_start_y, offset_start_z};
            offset_end = new int[3] {offset_end_x, offset_end_y, offset_end_z};
            DrawVoxelBufferChunk();
            dirty = false;
        }
        if (delay > 0) {
            delay--;
            return;
        }
        if (wavy_y) {
            frame +=1;
            if (frame == 1) {
                frame = 0;
                if (wavy_direction == 1) {
                    offset_end_y = (++offset_end_y % 16 );
                    if (offset_end_y == 15)
                        wavy_direction = -1;
                }
                else {
                    offset_end_y = --offset_end_y;
                    if (offset_end_y == 0)
                        wavy_direction = 1;
                }
                dirty = true;
            }
        }
        // else {
        //     int vox_prob = Random.Range(0,100);
        //     if (vox_prob == 1) {
        //         dirty=true;
        //     }
        // }
	}

    void MakeFace() {
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 1 ); //2
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4 + 3 ); //4
        faceCount++; // Add this line        
    }
 
    void MakeQuad(int[] x, int[] du, int[] dv, bool invert=false) {
        if (invert) {
            newVertices.Add(new Vector3 (x[0]+dv[0], x[1]+dv[1], x[2]+dv[2]));
            newVertices.Add(new Vector3 (x[0]+du[0]+dv[0], x[1]+du[1]+dv[1], x[2]+du[2]+dv[2]));
            newVertices.Add(new Vector3 (x[0]+du[0], x[1]+du[1], x[2]+du[2]));
            newVertices.Add(new Vector3 (x[0], x[1], x[2]));
        } else {
            newVertices.Add(new Vector3 (x[0], x[1], x[2]));
            newVertices.Add(new Vector3 (x[0]+du[0], x[1]+du[1], x[2]+du[2]));
            newVertices.Add(new Vector3 (x[0]+du[0]+dv[0], x[1]+du[1]+dv[1], x[2]+du[2]+dv[2]));
            newVertices.Add(new Vector3 (x[0]+dv[0], x[1]+dv[1], x[2]+dv[2]));
        }
        MakeFace();
    }

    void UpdateMesh ()
    {
        mesh.Clear ();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.colors = newColors.ToArray();
        mesh.Optimize ();
        mesh.RecalculateNormals ();
        col.sharedMesh=null;
        col.sharedMesh=mesh;

        newVertices.Clear();
        newTriangles.Clear(); 
        newColors.Clear();

        faceCount=0; //Fixed: Added this thanks to a bug pointed out by ratnushock!
    }

    void GreedyMeshing() {

        int vox1, vox2;
        int[] x = {0, 0, 0};
        int[] q = {0, 0, 0};

        // d is the direction we sweep in (x, y or z)
        for (int d=0; d<3; d++) {
            x[0] = 0;
            x[1] = 0;
            x[2] = 0;
            q[0] = 0;
            q[1] = 0;
            q[2] = 0;
            int u = (d+1)%3;
            int v = (d+2)%3;
            q[d] = 1;
            mask = new int[dims[u]][];
            for (int k = 0; k < dims[u]; k++) {
                mask[k] = new int[dims[v]];
            }
            for(x[d] = offset_start[d] - 1; x[d] < dims[d]-offset_end[d]; ) {
                // Compute mask
                for(x[v]=0+offset_start[v]; x[v] < dims[v]-offset_end[v]; x[v]++) {
                    for(x[u]=0+offset_start[u]; x[u] < dims[u]-offset_end[u]; x[u]++) {
                        vox1 = 0;
                        if (x[d] + starting[d] - offset_start[d] >= 0
                            && x[d] != offset_start[d] - 1
                            && voxel_buffer[start_x + x[0]][start_y + x[1]][start_z + x[2]] != 0
                           ) 
                        {
                            vox1 = voxel_buffer[start_x + x[0]][start_y + x[1]][start_z + x[2]];
                        }
                        vox2 = 0;
                        if (x[d] + starting[d] < buffer_size[d]-1
                            && x[d] != dims[d]-offset_end[d]-1
                            && voxel_buffer[start_x + x[0] + q[0]][start_y + x[1] + q[1]][start_z + x[2] + q[2]] != 0 )
                        {
                            vox2 = voxel_buffer[start_x + x[0] + q[0]][start_y + x[1] + q[1]][start_z + x[2] + q[2]];
                        }
                        if (vox1 != vox2) {
                            if (vox2 == 0)
                                mask[x[u]][x[v]] = vox1;
                            else if (vox1 == 0)
                                mask[x[u]][x[v]] = -vox2;
                        }
                    }
                }
            
                x[d]++;

                for (int j=0; j<dims[v]; j++) {
                    for (int i=0; i < dims[u]; ) {
                        if (mask[i][j] != 0) {
                            int w;
                            for (w=1; i+w<dims[u] && (mask[i+w][j] == mask[i][j]) ; ++w) {
                            }

                            // Compute the height of the quad
                            bool done = false;
                            int h;
                            for (h=1; j+h<dims[v]; h++) {
                                for(int k = 0; k<w; k++) {
                                    if(mask[i+k][j+h] != mask[i][j]) {
                                        done = true;
                                        break;
                                    }
                                }
                                if(done) {
                                    break;
                                }
                            }

                            // Create quad with the computed size

                            int[] du = {0, 0, 0};
                            int[] dv = {0, 0, 0};
                            x[u] = i;
                            x[v] = j;
                            int[] x_offset = {x[0]+start_x, x[1]+start_y, x[2]+start_z};
                            du[u] = w;
                            dv[v] = h;

                            MakeQuad(x_offset, du, dv, mask[i][j] < 0);
                            Color my_color = Color.red;
                            if (Mathf.Abs(mask[i][j]) == 2) {
                                my_color = Color.green;
                            }
                            newColors.Add(my_color);
                            newColors.Add(my_color);
                            newColors.Add(my_color);
                            newColors.Add(my_color);
                            // Zero out the already processed mask
                            for (int l = 0; l<h; l++) {
                                for (int k = 0; k<w; k++) {
                                    mask[i+k][j+l] = 0;
                                }
                            }

                            i+=w;
                        } else {
                            i++;
                        }
                    }
                }

            }
        }
    }

    void DrawVoxelBufferChunk() {
        GreedyMeshing();
        UpdateMesh();
    }
}
