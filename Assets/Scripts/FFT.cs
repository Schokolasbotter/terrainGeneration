using System.Collections;
using System.Collections.Generic;
using Numerics = System.Numerics;
using UnityEngine;

//https://youtu.be/v743U7gvLq0?si=sxJyAEAoc9gGkuqG 

public class FFT : MonoBehaviour
{
    public Mesh TerrainMesh;
    public Texture2D texture;
    private Vector3[] originalVertices, displacedVertices;
    private Vector2[] uvCoordinates;


    [Header("NoiseSettings")]
    private int Size = 128;

    private float squareMeshSize;

    public float heightMult = 1;
    public float noiseScale;

    public float[] newVertexHeights;
    // Start is called before the first frame update
    void Start()
    {
        TerrainMesh = GetComponent<MeshFilter>().mesh;
        GenerateNoise();

        originalVertices = TerrainMesh.vertices;
        uvCoordinates = TerrainMesh.uv;
        displacedVertices = new Vector3[originalVertices.Length];

        squareMeshSize = Mathf.Sqrt(TerrainMesh.vertexCount);

        Numerics.Complex[,] complex = FourierTransform();
        complex = ApplyFilter(complex, 0.1f);
        Numerics.Complex[,] inversedComplex = ApplyInverseFastFourierTransform(complex);
        newVertexHeights = ComplexToDouble(inversedComplex);


        for (int i = 0; i < originalVertices.Length; i++)
        {
            float height = (float)newVertexHeights[i];
            displacedVertices[i] = originalVertices[i] + Vector3.up * height * heightMult;
        }
        TerrainMesh.vertices = displacedVertices;
        TerrainMesh.RecalculateNormals();

        

        Texture2D outputTexture = DoubleArrayToTexture2D(newVertexHeights, (int)squareMeshSize);


        // Create a new material
        Material noiseMaterial = new Material(Shader.Find("Standard"));
        noiseMaterial.SetTexture("_MainTex", outputTexture);

        // Apply the material to the GameObject's Renderer
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = noiseMaterial;


      //  print(uvCoordinates.Length);
    }

    private void GenerateNoise()
    { 
        texture = new Texture2D(Size, Size);

        for(int x = 0; x<Size;  x++)
        {
            for (int y = 0; y < Size; y++)
            {
                float sample = Mathf.PerlinNoise((float)x*noiseScale, (float)y*noiseScale);
                Color color = new Color(sample,sample,sample);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
    }


    Texture2D DoubleArrayToTexture2D(float[] data, int size)
    {
        Texture2D texture = new Texture2D(size, size);

        for (int i = 0; i < data.Length; i++)
        {
            int x = i % size;
            int y = i / size;
            float value = data[i];

            // Normalize or scale value to [0, 1] if necessary
            // value = Mathf.Clamp01(value); // Uncomment this line if needed

            Color color = new Color(value, value, value); // Create grayscale color
            texture.SetPixel(x, y, color);
        }

        texture.Apply();
        return texture;
    }



    private Numerics.Complex[,] FourierTransform()
    {
        //Creates a new 2D array containing complexData (Numerics.Complex is structured like this ((double)Real, (double)Imaginary))
        Numerics.Complex[,] complexData = new Numerics.Complex[(int)squareMeshSize, (int)squareMeshSize];

        //for each pixel in the image, we get the greyscale value set to a value called 'intensity'
        //Then inside that position in the 2D array, we create a new complex number with the intensity as the double, and the imaginary number = 0

        //this function doesnt work, mismatch between uv and size of mesh
        for (int I = 0, X = 0; X < squareMeshSize; X++)
        {
            for (int Y = 0; Y < squareMeshSize; Y++, I++)
            {
                float intensity = texture.GetPixelBilinear(uvCoordinates[I].x, uvCoordinates[I].y).grayscale;
                complexData[X, Y] = new Numerics.Complex(intensity, 0);
            }
        }

        complexData = ApplyFastFourierTransform(complexData);
        return complexData;
    }

    private Numerics.Complex[,] ApplyFilter(Numerics.Complex[,] inputData, float r)
    {
        int n = (int)squareMeshSize;
        int m = (int)squareMeshSize;

        float halfN = n / 2f;
        float halfM = m / 2f;

        for (int u = 0; u < n; u++)
        {
            for (int v = 0; v < m; v++)
            {
                // Calculate frequency based on distance from the center
                float distance = Mathf.Sqrt(Mathf.Pow(u - halfN, 2) + Mathf.Pow(v - halfM, 2));
                float f = Mathf.Max(distance, 1); // Avoid division by zero

                // Apply the filter
                inputData[u, v] /= Mathf.Pow(f, r);
            }
        }

        return inputData;
    }

    private Numerics.Complex[,] ApplyFastFourierTransform(Numerics.Complex[,] inputData)
    {
        /*
        Our Equation provided https://web.williams.edu/Mathematics/sjmiller/public_html/hudson/Dickerson_Terrain.pdf

        F(u,v) = 1/NM {N−1∑x = 0}{M−1∑y = 0} -> f(x, y)e ^ (−2πi(xu / N + yu / M))

        F - the function
        u,v, the vertex

        N and M the dimensions of the input 2D image/Array

        f(x,y) the value of the current fragment

        (the double summation is represented with a nested loop)
        
        */

        double n = squareMeshSize;
        double m = squareMeshSize;
        Numerics.Complex[,] oComplex = new Numerics.Complex[(int)n,(int)m];



        //SUM ROWS AND SUM COLIMNS


        //for each fragment
        for (double u = 0; u < n; u++)
        {
            for (double v = 0; v < m; v++)
            {

                Numerics.Complex Sum = Numerics.Complex.Zero;

                for (double x = 0; x < n; x++)
                {
                    for (double y = 0; y < m; y++)
                    {
                        //As i (imaginary number) doesnt really exist in unity, we calculate the rest of teh exponent first, then when its added to the second part of the complex number, it is automatically imaginary
                        double exponent = -2f * Mathf.PI * (((x * u) / n)+ ((y*v)/m));
                        Sum += inputData[(int)x,(int) y] * Numerics.Complex.Exp(new Numerics.Complex(0, exponent));
                        print(Sum);
                    }
                }
                oComplex[(int)u, (int)v] = Sum/(n*m);
            }
        }



        return oComplex;
    }

    private Numerics.Complex[,] ApplyInverseFastFourierTransform(Numerics.Complex[,] inputData)
    {
        double n = squareMeshSize;
        double m = squareMeshSize;
        Numerics.Complex[,] oComplex = new Numerics.Complex[(int)n, (int)m];

        for (double u = 0; u < n; u++)
        {
            for (double v = 0; v < m; v++)
            {

                Numerics.Complex Sum = Numerics.Complex.Zero;

                for (double x = 0; x < n; x++)
                {
                    for (double y = 0; y < m; y++)
                    {
                        //As i (imaginary number) doesnt really exist in unity, we calculate the rest of teh exponent first, then when its added to the second part of the complex number, it is automatically imaginary
                        double exponent = 2f * Mathf.PI * (((x * u) / n) + ((y * v) / m));
                        Sum += inputData[(int)x, (int)y] * Numerics.Complex.Exp(new Numerics.Complex(0, exponent));
                    }
                }
                oComplex[(int)u, (int)v] = Sum;
            }
        }

        return oComplex;
    }


    private float[] ComplexToDouble(Numerics.Complex[,] complexData)
    {
        float[] oDoubleArray = new float[(int)squareMeshSize*(int)squareMeshSize];

        for(int x = 0, i = 0; x<squareMeshSize; x++)
        {
            for(int y = 0; y<squareMeshSize; y++,i++)
            {
                oDoubleArray[i] = (float)complexData[x, y].Real;
            }
        }

        return oDoubleArray;
    }
}
