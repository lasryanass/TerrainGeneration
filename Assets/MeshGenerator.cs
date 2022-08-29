using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;

    public float scale = 2f;

    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;


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
        /// the first code here was to make a simple quad instead of a grid (just to train)
        /*
        vertices = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(0,0,1),
            new Vector3(1,0,0),
            new Vector3(1,0,1)
        };
        triangles = new int[] {
            0, 1, 2,
            1,3, 2,
            // triangles can be shown only in clockwise order of points (backface culling)
        };
        */

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        //vertex count = (xSize + 1) * (zSize + 1)





        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for(int x = 0; x <= xSize; x++)
            {
                float y = DepthOctaveCalculate(x, z);
                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight)
                {
                    maxTerrainHeight = y;
                }
                if (y < minTerrainHeight)
                {
                    minTerrainHeight = y;
                }
                i++;
            }
        }
        triangles = new int[xSize * zSize * 6]; //amount of triangles we need
        int vert = 0;
        int tris = 0;
        for(int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // again, clockwise points 
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++;
                tris += 6;
            }
            vert++; 
            // in order to not make a triangle that has points in the left side of the mesh and the other at the right side
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }



    }

    float DepthOctaveCalculate(int x, int z)
    {
        float[] octaveFrequencies = new float[] { 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, 5.5f, 6};
        float[] octaveAmplitudes = new float[] { 1, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, 0.1f, 0 };
        float y = 0;
        for (int i = 0; i < octaveFrequencies.Length; i++)
        {
            y += octaveAmplitudes[i] * Mathf.PerlinNoise(octaveFrequencies[i] * x + .3f, octaveFrequencies[i] * z + .3f) * scale;
        }
        return y;
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }
      
}
