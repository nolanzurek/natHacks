using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    //number of tiles in grid
    public int xSize = 100;
    public int zSize = 100; 

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for(int i = 0, z = 0; z <= zSize; z++) {
            for(int x = 0; x <= xSize; x++) {
                float rawHeight = Mathf.PerlinNoise(x * .1f, z * .1f);
                float offsetHeight = Mathf.Max(0, (rawHeight) - 0.35f);
                float poweredHeight = offsetHeight * offsetHeight * offsetHeight * 15f;
                float minBoundedHeight = Mathf.Min(poweredHeight, .7f + .2f*Mathf.PerlinNoise(x*.5f + 1000, z*.5f + 1000));
                float roughHeight = minBoundedHeight + Mathf.PerlinNoise(x*.7f + 2000, z*.7f + 2000)*0.1f;
                float height = roughHeight;
                vertices[i] = new Vector3(x, height, z);
                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        triangles = new int[xSize * zSize * 6];

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
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        
        // mesh collider
        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}
