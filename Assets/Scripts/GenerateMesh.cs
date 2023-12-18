using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    public int xSize;
    public int ySize;

    private Mesh mesh;
    private Vector3[] vertices;


    // Start is called before the first frame update
    void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Plane2";

        vertices = new Vector3[(xSize+1) * (ySize+1)];

        for(int i = 0, y=0; y<=ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, 0, y);
            }
        }

        mesh.vertices = vertices;

        int[] triangles = new int[xSize * ySize * 6];

        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int x=0,i=0; x<=xSize; x++)
        {
            for(int y=0;y<=ySize;y++,i++)
            {
                uvs[i] = new Vector2((float)x / (xSize + 1), (float)y / (ySize + 1));

            }
        }

        mesh.SetUVs(0,uvs);
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
