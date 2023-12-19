using System.Collections;
using System.Collections.Generic;
using Numerics = System.Numerics;
using UnityEngine;

//https://youtu.be/v743U7gvLq0?si=sxJyAEAoc9gGkuqG 

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

        FourierTransform();

        // Create a new material
        Material noiseMaterial = new Material(Shader.Find("Standard"));
        noiseMaterial.SetTexture("_MainTex", texture);

        // Apply the material to the GameObject's Renderer
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = noiseMaterial;


        print(uvCoordinates.Length);
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

    private void FourierTransform()
    {
        //Creates a new 2D array containing complexData (Numerics.Complex is structured like this ((double)Real, (double)Imaginary))
        Numerics.Complex[,] complexData = new Numerics.Complex[Size, Size];

        //for each pixel in the image, we get the greyscale value set to a value called 'intensity'
        //Then inside that position in the 2D array, we create a new complex number with the intensity as the double, and the imaginary number = 0




        //this function doesnt work, mismatch between uv and size of mesh
        for (int I = 0, X = 0; I<uvCoordinates.Length; X++)
        {
            for (int Y = 0; Y < Size; Y++, I++)
            {
                float intensity = texture.GetPixelBilinear(uvCoordinates[I].x, uvCoordinates[I].y).grayscale;
                complexData[X, Y] = new Numerics.Complex(intensity, 0);

                print(I);
            }
        }


        //Apply The FFT

     //   complexData = ApplyFastFourierTransform(complexData);
    }

    private Numerics.Complex[,] ApplyFastFourierTransform(Numerics.Complex[,] inputData)
    {
        /*
        Our Equation provided https://web.williams.edu/Mathematics/sjmiller/public_html/hudson/Dickerson_Terrain.pdf

        F(u,v) = 1/NM {N−1∑x = 0}{M−1∑y = 0} -> f(x, y)e ^ (−2πi(xu / N + yu / M))

        F - the function
        u,v, the fragment

        N and M the dimensions of the input 2D image/Array

        f(x,y) the value of the current fragment

        (the double summation is represented with a nested loop)
        
        */

        int n = Size;
        int m = Size;
        Numerics.Complex[,] oComplex = new Numerics.Complex[n,m];


        //for each fragment
        for (int u = 0; u <= n; u++)
        {
            for (int v = 0; v <= m; v++)
            {

                Numerics.Complex Sum = Numerics.Complex.Zero;

                for (int x = 0; x <= n; x++)
                {
                    for (int y = 0; y <= m; y++)
                    {
                        //As i (imaginary number) doesnt really exist in unity, we calculate the rest of teh exponent first, then when its added to the second part of the complex number, it is automatically imaginary
                        double a = -2 * Mathf.PI * ((x * u / (double)n)+ (y*v*(double)m));
                        Sum += inputData[x, y] * Numerics.Complex.Exp(new Numerics.Complex(0, a));
                    }
                }
                oComplex[u, v] = Sum;
            }
        }

        return inputData;
    }
}
