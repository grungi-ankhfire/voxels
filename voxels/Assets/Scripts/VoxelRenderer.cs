using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelRenderer : MonoBehaviour {

    private Voxel[][][] voxel_buffer;
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

    public int[] dims;
    public int[] buffer_size;
    public int[] starting;
    public int[] offset_start;
    public int[] offset_end;

    // Testing stuff :p
    public bool wavy_y = false;
    public int delay = 0;
    public int frame = 0;
    private int wavy_direction = 1;
    // End testing stuff


    public List<Vector3> newVertices = new List<Vector3>();
    public List<Color32> newColors = new List<Color32>();
    public List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();
  
    private float tUnit = 0.25f;
      
    private Mesh mesh;
    private MeshCollider col;
  
    private int faceCount;

    private MaskData[][] mask;

    public bool dirty = false;

    private Color32[] palette;

    private class MaskData
    {
        public Color32 color;
        public bool reverse;
        public int id;

        public MaskData(int id_new, bool rev) {
            id = id_new;
            reverse = rev;
        }

        public bool Equals(MaskData other)
        {
            // If parameter is null return false:
            if (other == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (id == other.id && reverse == other.reverse);
        }
    }

	// Use this for initialization
	void Start () {
        //voxel_buffer = VoxelBuffer.instance.voxel_buffer;

        VoxelVolume volume = transform.parent.gameObject.GetComponent<VoxelVolume>();
        voxel_buffer = volume.voxels;
        palette = volume.palette.colors;

        dims = new int[3] {chunk_x_size, chunk_y_size, chunk_z_size};
        buffer_size = volume.size;
        starting = new int[3] {start_x, start_y, start_z};
        offset_start = new int[3] {offset_start_x, offset_start_y, offset_start_z};
        offset_end = new int[3] {offset_end_x, offset_end_y, offset_end_z};

        mesh = GetComponent<MeshFilter> ().mesh;
        mesh.MarkDynamic();
        col = GetComponent<MeshCollider> ();

        DrawVoxelBufferChunk();
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
        mesh.colors32 = newColors.ToArray();
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

        Voxel vox1, vox2;
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
            mask = new MaskData[dims[u]][];
            for (int k = 0; k < dims[u]; k++) {
                mask[k] = new MaskData[dims[v]];
            }
            for(x[d] = offset_start[d] - 1; x[d] < dims[d]-offset_end[d]; ) {
                // Compute mask
                for(x[v]=0+offset_start[v]; x[v] < dims[v]-offset_end[v]; x[v]++) {
                    for(x[u]=0+offset_start[u]; x[u] < dims[u]-offset_end[u]; x[u]++) {
                        vox1 = null;
                        if (x[d] + starting[d] - offset_start[d] >= 0
                            && x[d] != offset_start[d] - 1
                            && voxel_buffer[start_x + x[0]][start_y + x[1]][start_z + x[2]] != null
                           ) 
                        {
                            vox1 = voxel_buffer[start_x + x[0]][start_y + x[1]][start_z + x[2]];
                        }
                        vox2 = null;
                        if (x[d] + starting[d] < buffer_size[d]-1
                            && x[d] != dims[d]-offset_end[d]-1
                            && voxel_buffer[start_x + x[0] + q[0]][start_y + x[1] + q[1]][start_z + x[2] + q[2]] != null )
                        {
                            vox2 = voxel_buffer[start_x + x[0] + q[0]][start_y + x[1] + q[1]][start_z + x[2] + q[2]];
                        }
                        if (vox1 != vox2) {
                            if (vox2 == null)
                                mask[x[u]][x[v]] = new MaskData(vox1.id-1, false);
                            else if (vox1 == null)
                                mask[x[u]][x[v]] = new MaskData(vox2.id-1, true);
                        }
                    }
                }
            
                x[d]++;

                for (int j=0; j<dims[v]; j++) {
                    for (int i=0; i < dims[u]; ) {
                        if (mask[i][j] != null) {
                            int w;
                            for (w=1; (i+w<dims[u])
                                      && mask[i][j].Equals(mask[i+w][j]) ; ++w) {
                            }

                            // Compute the height of the quad
                            bool done = false;
                            int h;
                            for (h=1; j+h<dims[v]; h++) {
                                for(int k = 0; k<w; k++) {
                                    if( !mask[i][j].Equals(mask[i+k][j+h]) ) {
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
                            du[u] = w;
                            dv[v] = h;

                            MakeQuad(x, du, dv, mask[i][j].reverse);
                            Color32 my_color = palette[mask[i][j].id];
                            newColors.Add(my_color);
                            newColors.Add(my_color);
                            newColors.Add(my_color);
                            newColors.Add(my_color);
                            // Zero out the already processed mask
                            for (int l = 0; l<h; l++) {
                                for (int k = 0; k<w; k++) {
                                    mask[i+k][j+l] = null;
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
        offset_start = new int[3] {offset_start_x, offset_start_y, offset_start_z};
        offset_end = new int[3] {offset_end_x, offset_end_y, offset_end_z};
        for (int i = 0; i < 3; i++) {
            if (offset_start[i] + offset_end[i] > dims[i]) {
                return;
            }
        }
        GreedyMeshing();
        UpdateMesh();
    }

    public void SetVanishVoxelOffset(int fraction, Vector3 direction) {
        int new_offset;
        if (direction.x > 0) {
            if (fraction > start_x) {
                new_offset = Mathf.Max(Mathf.Min(fraction-start_x, 16), 0);
                if (offset_start_x != new_offset) {
                    offset_start_x = new_offset;
                    DrawVoxelBufferChunk();
                }
            }
        } else if (direction.x < 0) {
            if (128-fraction > start_x) {
                new_offset = chunk_x_size - Mathf.Max(Mathf.Min(64-fraction-start_x, 16), 0);
                if (offset_end_x != new_offset) {
                    offset_end_x = new_offset;
                    DrawVoxelBufferChunk();
                }
            }
        }
    }

    public void SetAppearVoxelOffset(int fraction, Vector3 direction) {
        int new_offset;
        if (direction.x > 0) {
            if (fraction > start_x) {
                new_offset = chunk_x_size - Mathf.Max(Mathf.Min(fraction-start_x, 16), 0);
                if (offset_end_x != new_offset) {
                    offset_end_x = new_offset;
                    DrawVoxelBufferChunk();
                }
            }
        } else if (direction.x < 0) {
            if (128-fraction > start_x) {
                new_offset = Mathf.Max(Mathf.Min(64-fraction-start_x, 16), 0);
                if (offset_start_x != new_offset) {
                    offset_start_x = new_offset;
                    DrawVoxelBufferChunk();
                }
            }
        }
    }

}
