using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelChunkRenderer : MonoBehaviour {

    public VoxelChunk active_chunk;
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

    void DrawVoxelBufferChunk() {
        for (int x = start_x; x < start_x + chunk_x_size; x++) {
            for (int y = start_y; y < start_y + chunk_y_size; y++) {
                for (int z = start_z; z < start_z + chunk_z_size; z++) {
                    if (active_chunk.voxels[x,y,z] == 0) continue;
                    if (y == active_chunk.y_size-1 || active_chunk.voxels[x,y+1,z] == 0) {
                        CubeTop(x,y,z,0);
                    }
                    if (y == 0 || active_chunk.voxels[x,y-1,z] == 0) {
                        CubeBot(x,y,z,0);
                    }
                    if (x == 0 || active_chunk.voxels[x-1,y,z] ==0) {
                        CubeWest(x,y,z,0);
                    }
                    if (x == active_chunk.x_size-1 || active_chunk.voxels[x+1,y,z] ==0) {
                        CubeEast(x,y,z,0);
                    }
                    if (z == 0 || active_chunk.voxels[x,y,z-1] ==0) {
                        CubeSouth(x,y,z,0);
                    }
                    if (z == active_chunk.z_size-1 || active_chunk.voxels[x,y,z+1] ==0) {
                        CubeNorth(x,y,z,0);
                    }
                }
            }
        }
        UpdateMesh();
    }
}
