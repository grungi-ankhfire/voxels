using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelRenderer : MonoBehaviour {

    private int[,,] voxel_buffer;
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

    void MakeQuad(int[] x, int[] du, int[] dv) {
        newVertices.Add(new Vector3 (x[0], x[1], x[2]));
        newVertices.Add(new Vector3 (x[0]+du[0], x[1]+du[1], x[2]+du[2]));
        newVertices.Add(new Vector3 (x[0]+du[0]+dv[0], x[1]+du[1]+dv[1], x[2]+du[2]+dv[2]));
        newVertices.Add(new Vector3 (x[0]+dv[0], x[1]+dv[1], x[2]+dv[2]));
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

        // d is the direction we sweep in (x, y or z)
        for (int d=0; d<3; d++) {
            int[] x = {0, 0, 0};
            int[] q = {0, 0, 0};
            int u = (d+1)%3;
            int v = (d+2)%3;
            bool[,] mask = new bool[dims[u], dims[v]];
            q[d] = 1;
            for(x[d]=-1; x[d] < dims[d]; ) {
                // Compute mask
                for(x[v]=0; x[v] < dims[v]; x[v]++) {
                    for(x[u]=0; x[u] < dims[v]; x[u]++) {
                        bool vox1 = false;
                        if ( x[d] >= 0 && voxel_buffer[start_x + x[0], start_y + x[1], start_z + x[2]] != 0 ) {
                            vox1 = true;
                        }
                        bool vox2 = false;
                        if( x[d] < dims[d]-1 && voxel_buffer[start_x + x[0] + q[0], start_y + x[1] + q[1], start_z + x[2] + q[2]] != 0 ) {
                            vox2 = true;
                        }                        
                        mask[x[u], x[v]] = (vox1 != vox2);
                    }
                }
            
                x[d]++;

                for (int j=0; j<dims[v]; j++) {
                    for (int i=0; i < dims[u]; ) {
                        if (mask[i,j]) {
                            int w;
                            for (w=1; i+w<dims[u] && mask[i+w,j] ; ++w) {
                            }

                            // Compute the height of the quad
                            bool done = false;
                            int h;
                            for (h=1; j+h<dims[v]; h++) {
                                for(int k = 0; k<w; k++) {
                                    if(!mask[i+k,j+h]) {
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
                            x[0] += start_x;
                            x[1] += start_y;
                            x[2] += start_z;
                            du[u] = w;
                            dv[v] = h;

                            MakeQuad(x, du, dv);

                            // Zero out the already processed mask
                            for (int l = 0; l<h; l++) {
                                for (int k = 0; k<w; k++) {
                                    mask[i+k,j+l] = false;
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
