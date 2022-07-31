using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    //initilize the mesh, as well as the triangles and vertices that will be used to construct the mesh
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector3[] bumps;

    //initilize the vertex color storage for procedural terrain coloring
    Color[] colors;
    public Gradient gradient;

    //number of tiles in grid is 100
    public int xSize = 100;
    public int zSize = 100; 

    //setting default values for the min and max terrain height
    //these will be updated later when the terrain is generated
    float minTerrainHeight = 1000;
    float maxTerrainHeight = 0;

    //sample data
    int[] sampleData = new int[] {1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1, 2, 2};
    float[] sampleDataSmooth;

    // Start is called before the first frame update
    void Start()
    {

        sampleDataSmooth = new float[sampleData.Length];

        for(int i = 0; i < sampleData.Length; i++) {

            float total = 0;
            for(int j = i - 10; j < i + 10; j++) {
                total += sampleData[(j + sampleData.Length)%(sampleData.Length)];
            }
            sampleDataSmooth[i] = total/20f;

        }

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //create the mesh
        CreateShape();

        //update the mesh as the game runs
        UpdateMesh();

    }

    void CreateShape()
    {

        //array contains all the vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //looping over all the vertices...
        for(int i = 0, z = 0; z <= zSize; z++) {
            for(int x = 0; x <= xSize; x++) {
                //raw perlin noise
                float rawHeight = Mathf.PerlinNoise(x * .1f, z * .1f);
                //offsets the height in order to apply a non-linear transformation to it
                float offsetHeight = Mathf.Max(0, (rawHeight) - 0.4f);
                //raises the height to a power, which has the effect of exaggerating peaks and flattening low areas
                float poweredHeight = offsetHeight * offsetHeight * offsetHeight * 15f;
                //creates a minimum terrain height
                float minBoundedHeight = Mathf.Min(poweredHeight, .7f + .2f*Mathf.PerlinNoise(x*.5f + 1000, z*.5f + 1000));
                //adds texture to the terrain by adding an additional layer of perin noise at a higher frequency
                float roughHeight = 2f*minBoundedHeight + Mathf.PerlinNoise(x*.7f + 2000, z*.7f + 2000)*0.1f;
                float height = roughHeight*3f;

                //sets the current vertex
                vertices[i] = new Vector3(x, height, z);

                //updates the minimum and maximum height of the whole terrain based on the new vertex
                if(height > maxTerrainHeight) {
                    maxTerrainHeight = height;
                } else if (height < minTerrainHeight) {
                    minTerrainHeight = height;
                }

                //iterates vertex count
                i++;


            }
        }

        bumps = new Vector3[(xSize + 1) * (zSize + 1)];

        for(int i = 0, z = 0; z <= zSize; z++) {
            for(int x = 0; x <= xSize; x++) {
                bumps[i] = new Vector3(x, Mathf.PerlinNoise(x*.7f + 2000, z*.7f + 2000)*0.1f, z);
                i++;
            }
        }

        //number of vertices
        int vert = 0;
        //number of triangles
        int tris = 0;
        //array that contains all the triangles in the mesh
        triangles = new int[xSize * zSize * 6];

        //for each vertex, the two adjacent triangles are added to the mesh
        for(int z = 0; z < zSize; z++) {

            for(int x = 0; x < xSize; x++) {
                
                triangles[0 + tris] = vert + 0;
                triangles[1 + tris] = vert + xSize+1;
                triangles[2 + tris] = vert + 1;
                triangles[3 + tris] = vert + 1;
                triangles[4 + tris] = vert + xSize+1;
                triangles[5 + tris] = vert + xSize+2; 

                vert++;
                tris+=6;
            }
            vert++;
        }
        
        //array that stores all the colors of the vertices
        colors = new Color[vertices.Length];

        //looping over all the vertices
        for(int i = 0, z = 0; z <= zSize; z++) {
            for(int x = 0; x <= xSize; x++) {
                //normalizes height of vertex over the terrain height range
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                //colors the vertex based on the gradient
                colors[i] = gradient.Evaluate(height);
                //increments the vertex count
                i++;
            }
        }

    }

    //updates to the mesh each frame
    void UpdateMesh()
    {
        mesh.Clear();

        Vector3[] heightTemp = new Vector3[(xSize + 1) * (zSize + 1)];

        for(int i = 0, z = 0; z <= zSize; z++) {
            for(int x = 0; x <= xSize; x++) {
                heightTemp[i] = vertices[i] + (bumps[i] * sampleDataSmooth[((int)(Time.frameCount/12))%(sampleDataSmooth.Length)] * 4);
                i++;
            }
        }

        mesh.vertices = heightTemp;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();

        // mesh collider
        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    void Update() {
        UpdateMesh();
    }
    

}
