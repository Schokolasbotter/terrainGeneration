using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FFT : MonoBehaviour
{
    public Mesh TerrainMesh;
    private Texture2D texture;
    private Vector3[] originalVertices, displacedVertices;
    private Vector2[] uvCoordinates;


    [Header("NoiseSettings")]
    private int Size = 256;

    public float heightMult = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        TerrainMesh = GetComponent<MeshFilter>().mesh;
        print(TerrainMesh.vertexCount);
        GenerateNoise();

        originalVertices = TerrainMesh.vertices;
        uvCoordinates = TerrainMesh.uv;
        displacedVertices = new Vector3[originalVertices.Length];

        for(int i = 0; i<originalVertices.Length;i++)
        {
            float height = texture.GetPixelBilinear(uvCoordinates[i].x, uvCoordinates[i].y).grayscale;
            displacedVertices[i] = originalVertices[i] + Vector3.up * height * heightMult;
        }
        TerrainMesh.vertices = displacedVertices;
        TerrainMesh.RecalculateNormals();



        // Create a new material
        Material noiseMaterial = new Material(Shader.Find("Standard"));
        noiseMaterial.SetTexture("_MainTex", texture);

        // Apply the material to the GameObject's Renderer
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = noiseMaterial;

    }

    private void GenerateNoise()
    {
        texture = new Texture2D(Size, Size);

        for(int x = 0; x<Size;  x++)
        {
            for (int y = 0; y < Size; y++)
            {
                float sample = Mathf.PerlinNoise((float)x / Size, (float)y / Size);
                Color color = new Color(sample,sample,sample);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
    }

    private void AlterGeometry()
    {

    }
}
