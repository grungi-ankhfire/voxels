using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelGenerator : MonoBehaviour {

  
    public   List<Vector3> newVertices = new List<Vector3>();
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
  
        CubeTop(0,0,0,0);
        CubeNorth(0,0,0,0);
        CubeWest(0,0,0,0);
        CubeSouth(0,0,0,0);
        CubeEast(0,0,0,0);
        CubeBot(0,0,0,0);
        UpdateMesh ();
    }

	
	// Update is called once per frame
	void Update () {
        //UpdateMesh ();
	}

    void CubeTop (int x, int y, int z, byte block) {
      
        Debug.Log("Creating cube top");
        newVertices.Add(new Vector3 (x,  y,  z + 1));
        newVertices.Add(new Vector3 (x + 1, y,  z + 1));
        newVertices.Add(new Vector3 (x + 1, y,  z ));
        newVertices.Add(new Vector3 (x,  y,  z ));

        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 1 ); //2
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4 + 3 ); //4
        faceCount++; // Add this line
    }


    void CubeNorth (int x, int y, int z, byte block) {     
        //CubeNorth
        newVertices.Add(new Vector3 (x + 1, y-1, z + 1));
        newVertices.Add(new Vector3 (x + 1, y, z + 1));
        newVertices.Add(new Vector3 (x, y, z + 1));
        newVertices.Add(new Vector3 (x, y-1, z + 1));
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 1 ); //2
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4 + 3 ); //4
        faceCount++; // Add this line
    }

    void CubeEast (int x, int y, int z, byte block) {
        //CubeEast
        newVertices.Add(new Vector3 (x + 1, y - 1, z));
        newVertices.Add(new Vector3 (x + 1, y, z));
        newVertices.Add(new Vector3 (x + 1, y, z + 1));
        newVertices.Add(new Vector3 (x + 1, y - 1, z + 1));
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 1 ); //2
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4 + 3 ); //4
        faceCount++; // Add this line
    }

    void CubeSouth (int x, int y, int z, byte block) {
        //CubeSouth
        newVertices.Add(new Vector3 (x, y - 1, z));
        newVertices.Add(new Vector3 (x, y, z));
        newVertices.Add(new Vector3 (x + 1, y, z));
        newVertices.Add(new Vector3 (x + 1, y - 1, z));
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 1 ); //2
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4 + 3 ); //4
        faceCount++; // Add this line
    }

    void CubeWest (int x, int y, int z, byte block) {     
        //CubeWest
        newVertices.Add(new Vector3 (x, y- 1, z + 1));
        newVertices.Add(new Vector3 (x, y, z + 1));
        newVertices.Add(new Vector3 (x, y, z));
        newVertices.Add(new Vector3 (x, y - 1, z));
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 1 ); //2
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4 + 3 ); //4
        faceCount++; // Add this line
    }

    void CubeBot (int x, int y, int z, byte block) {  
        //CubeBot
        newVertices.Add(new Vector3 (x,  y-1,  z ));
        newVertices.Add(new Vector3 (x + 1, y-1,  z ));
        newVertices.Add(new Vector3 (x + 1, y-1,  z + 1));
        newVertices.Add(new Vector3 (x,  y-1,  z + 1));
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 1 ); //2
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4  ); //1
        newTriangles.Add(faceCount * 4 + 2 ); //3
        newTriangles.Add(faceCount * 4 + 3 ); //4
        faceCount++; // Add this line
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

}
