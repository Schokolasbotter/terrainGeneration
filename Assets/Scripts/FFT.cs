using System.Collections;
using System.Collections.Generic;
using Numerics = System.Numerics;
using UnityEngine;

//https://youtu.be/v743U7gvLq0?si=sxJyAEAoc9gGkuqG 

public class FFT : MonoBehaviour
{
    public Mesh TerrainMesh;
    public Texture2D texture;
    private Vector3[] originalVertices;
    public Vector3[] heightmap1;
    public float[] heightmap;
    private Vector2[] uvCoordinates;
    public float filter;

    [Header("NoiseSettings")]
    private int Size = 128;

    private float squareMeshSize;

    public float heightMult = 30;
    public float noiseScale;
    public float noiseScale2;

    public float[] newVertexHeights;
    // Start is called before the first frame update

    public void regenerateNoiseScale()
    {
        noiseScale = Random.Range(1, 3);
        noiseScale2 = Random.Range(3, 6);
    }
    void Start()
    {
        TerrainMesh = GetComponent<MeshFilter>().mesh;
        RebuildHeightMap();
    }
    public void RebuildHeightMap()
    {
        GenerateNoise();

        originalVertices = TerrainMesh.vertices;
        uvCoordinates = TerrainMesh.uv;
        heightmap1 = new Vector3[originalVertices.Length];
        heightmap = new float[originalVertices.Length];

        squareMeshSize = Mathf.Sqrt(TerrainMesh.vertexCount);

        Numerics.Complex[,] complex = FourierTransform();
        complex = ApplyFilter(complex, filter);
        Numerics.Complex[,] inversedComplex = ApplyInverseFastFourierTransform(complex);
        newVertexHeights = ComplexToDouble(inversedComplex);


        for (int i = 0; i < originalVertices.Length; i++)
        {
            float height = (float)newVertexHeights[i];
            heightmap1[i] = originalVertices[i] + Vector3.up * height * heightMult;

            heightmap[i] = heightmap1[i].y;
        }
    }

    private void GenerateNoise()
    { 
        texture = new Texture2D(Size, Size);

        for(int x = 0; x<Size;  x++)
        {
            for (int y = 0; y < Size; y++)
            {
                float sample1 = Mathf.PerlinNoise(noiseScale * (float)x/Size,noiseScale * (float)y/Size );
                float sample2 = Mathf.PerlinNoise(2435.3426f + noiseScale2 * (float)x/Size, 159232.766f + noiseScale2 * (float)y/Size );
                float sample3 = Mathf.PerlinNoise(2435.3426f + 0.5f * (float)x/Size, 159232.766f + 0.5f * (float)y/Size );
                float combinedSample = (sample1 + sample2 + sample3) / 3.0f;
                combinedSample = combinedSample * combinedSample * (3.0f - 2.0f * combinedSample); // Contrast adjustment
                combinedSample = Mathf.Clamp(combinedSample, 0.0f, 1.0f);


                Color color = new Color(combinedSample, combinedSample, combinedSample);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
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
        

        Since this is a direct implementation of the DFT algorithm, the time complexity is O(n) 


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
                        //print(Sum);
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
