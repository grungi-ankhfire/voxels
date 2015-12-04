using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelRenderer : MonoBehaviour {

    private int[,,] voxel_buffer;
    private int buffer_x_size;
    private int buffer_y_size;
    private int buffer_z_size;
    public int chunk_x_size = 16;
    public int chunk_y_size = 16;
    public int chunk_z_size = 16;

    public int start_x = 0;
    public int start_y = 0;
    public int start_z = 0;

    public List<Vector3> newVertices = new List<Vector3>();
    public List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();
  
    private float tUnit = 0.25f;
      
    private Mesh mesh;
    private MeshCollider col;
  
    private int faceCount;

	// Use this for initialization
	void Start () {
        voxel_buffer = VoxelBuffer.instance.voxel_buffer;
        buffer_x_size = VoxelBuffer.instance.buffer_x_size;
        buffer_y_size = VoxelBuffer.instance.buffer_y_size;
        buffer_z_size = VoxelBuffer.instance.buffer_z_size;

        mesh = GetComponent<MeshFilter> ().mesh;
        col = GetComponent<MeshCollider> ();

        DrawVoxelBufferChunk();
	}

	// Update is called once per frame
	void Update () {
	
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

    void CubeTop (int x, int y, int z, byte block) {
        newVertices.Add(new Vector3 (x,  y,  z + 1));
        newVertices.Add(new Vector3 (x + 1, y,  z + 1));
        newVertices.Add(new Vector3 (x + 1, y,  z ));
        newVertices.Add(new Vector3 (x,  y,  z ));
        MakeFace();
    }

    void CubeNorth (int x, int y, int z, byte block) {     
        newVertices.Add(new Vector3 (x + 1, y-1, z + 1));
        newVertices.Add(new Vector3 (x + 1, y, z + 1));
        newVertices.Add(new Vector3 (x, y, z + 1));
        newVertices.Add(new Vector3 (x, y-1, z + 1));
        MakeFace();
    }

    void CubeEast (int x, int y, int z, byte block) {
        newVertices.Add(new Vector3 (x + 1, y - 1, z));
        newVertices.Add(new Vector3 (x + 1, y, z));
        newVertices.Add(new Vector3 (x + 1, y, z + 1));
        newVertices.Add(new Vector3 (x + 1, y - 1, z + 1));
        MakeFace();
    }

    void CubeSouth (int x, int y, int z, byte block) {
        newVertices.Add(new Vector3 (x, y - 1, z));
        newVertices.Add(new Vector3 (x, y, z));
        newVertices.Add(new Vector3 (x + 1, y, z));
        newVertices.Add(new Vector3 (x + 1, y - 1, z));
        MakeFace();
    }

    void CubeWest (int x, int y, int z, byte block) {     
        newVertices.Add(new Vector3 (x, y- 1, z + 1));
        newVertices.Add(new Vector3 (x, y, z + 1));
        newVertices.Add(new Vector3 (x, y, z));
        newVertices.Add(new Vector3 (x, y - 1, z));
        MakeFace();
    }

    void CubeBot (int x, int y, int z, byte block) {  
        newVertices.Add(new Vector3 (x,  y-1,  z ));
        newVertices.Add(new Vector3 (x + 1, y-1,  z ));
        newVertices.Add(new Vector3 (x + 1, y-1,  z + 1));
        newVertices.Add(new Vector3 (x,  y-1,  z + 1));
        MakeFace();
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
        mesh.Optimize ();
        mesh.RecalculateNormals ();

        col.sharedMesh=null;
        col.sharedMesh=mesh;

        newVertices.Clear();
        newTriangles.Clear(); 

        faceCount=0; //Fixed: Added this thanks to a bug pointed out by ratnushock!
    }

    void GreedyMeshing() {

        int[] dims = {chunk_x_size, chunk_y_size, chunk_z_size};
        int[] buffer_size = {buffer_x_size, buffer_y_size, buffer_z_size};
        int[] starting = {start_x, start_y, start_z};

        // d is the direction we sweep in (x, y or z)
        for (int d=0; d<3; d++) {
            int[] x = {0, 0, 0};
            int[] q = {0, 0, 0};
            int u = (d+1)%3;
            int v = (d+2)%3;
            int[,] mask = new int[dims[u], dims[v]];
            q[d] = 1;
            for(x[d]=-1; x[d] < dims[d]; ) {
                // Compute mask
                for(x[v]=0; x[v] < dims[v]; x[v]++) {
                    for(x[u]=0; x[u] < dims[v]; x[u]++) {
                        int vox1 = 0;
                        if ( x[d] + starting[d] >= 0 && voxel_buffer[start_x + x[0], start_y + x[1], start_z + x[2]] != 0 ) {
                            vox1 = voxel_buffer[start_x + x[0], start_y + x[1], start_z + x[2]];
                        }
                        int vox2 = 0;
                        if( x[d] + starting[d] < buffer_size[d]-1 && voxel_buffer[start_x + x[0] + q[0], start_y + x[1] + q[1], start_z + x[2] + q[2]] != 0 ) {
                            vox2 = voxel_buffer[start_x + x[0] + q[0], start_y + x[1] + q[1], start_z + x[2] + q[2]];
                        }
                        if (vox1 != vox2) {
                            if (vox2 == 0)
                                mask[x[u], x[v]] = vox1;
                            else if (vox1 == 0)
                                mask[x[u], x[v]] = -vox2;
                        }
                    }
                }
            
                x[d]++;

                for (int j=0; j<dims[v]; j++) {
                    for (int i=0; i < dims[u]; ) {
                        if (mask[i,j] != 0) {
                            int w;
                            for (w=1; i+w<dims[u] && (mask[i+w,j] == mask[i,j]) ; ++w) {
                            }

                            // Compute the height of the quad
                            bool done = false;
                            int h;
                            for (h=1; j+h<dims[v]; h++) {
                                for(int k = 0; k<w; k++) {
                                    if(mask[i+k,j+h] != mask[i,j]) {
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

                            MakeQuad(x_offset, du, dv, mask[i,j] < 0);

                            // Zero out the already processed mask
                            for (int l = 0; l<h; l++) {
                                for (int k = 0; k<w; k++) {
                                    mask[i+k,j+l] = 0;
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
        // for (int x = start_x; x < start_x + chunk_x_size; x++) {
        //     for (int y = start_y; y < start_y + chunk_y_size; y++) {
        //         for (int z = start_z; z < start_z + chunk_z_size; z++) {
        //             if (voxel_buffer[x,y,z] == 0) continue;
        //             if (y == VoxelBuffer.instance.buffer_y_size-1 || voxel_buffer[x,y+1,z] == 0) {
        //                 CubeTop(x,y,z,0);
        //             }
        //             if (y == 0 || voxel_buffer[x,y-1,z] == 0) {
        //                 CubeBot(x,y,z,0);
        //             }
        //             if (x == 0 || voxel_buffer[x-1,y,z] ==0) {
        //                 CubeWest(x,y,z,0);
        //             }
        //             if (x == VoxelBuffer.instance.buffer_x_size-1 || voxel_buffer[x+1,y,z] ==0) {
        //                 CubeEast(x,y,z,0);
        //             }
        //             if (z == 0 || voxel_buffer[x,y,z-1] ==0) {
        //                 CubeSouth(x,y,z,0);
        //             }
        //             if (z == VoxelBuffer.instance.buffer_z_size-1 || voxel_buffer[x,y,z+1] ==0) {
        //                 CubeNorth(x,y,z,0);
        //             }
        //         }
        //     }
        // }
        GreedyMeshing();
        UpdateMesh();
    }
}
