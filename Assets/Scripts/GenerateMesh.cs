using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    [Header("2^N + 1")]
    public int N;
    private int xSize,ySize;

    private Mesh mesh;
    private Vector3[] vertices;


    // Start is called before the first frame update
    void Awake()
    {
        xSize = ySize = (int)Mathf.Pow(2,N)+1;
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Plane2";

        vertices = new Vector3[xSize * ySize];

        for(int i = 0, y=0; y<ySize; y++)
        {
            for (int x = 0; x < xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, 0, y);
            }
        }

        mesh.vertices = vertices;
        int[] triangles = new int[xSize * (ySize-1) * 6];

        for (int ti = 0, vi = 0, y = 0; y < (ySize-1); y++, vi++)
        {
            for (int x = 0; x < (xSize-1); x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + (xSize-1) + 1;
                triangles[ti + 5] = vi + (xSize-1) + 2;
            }
        }

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int x=0,i=0; x<xSize; x++)
        {
            for(int y=0;y<ySize;y++,i++)
            {
                
                uvs[i] = new Vector2((float)x / (xSize), (float)y / (ySize));

            }
        }

        mesh.SetUVs(0,uvs);
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
