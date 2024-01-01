using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class diamondSquare : MonoBehaviour
{
    public float height = 30f;
    [Range(0f, 2f)] public float smoothness;
    private float sideSize;
    Mesh mesh;
    Vector3[] vertices;
    public float[] heightmap;
    int maximumIndexCoordinate = 0;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        sideSize = Mathf.Sqrt(mesh.vertices.Length);
        maximumIndexCoordinate = (int)sideSize - 1;
        vertices = mesh.vertices;
        heightmap = new float[vertices.Length];

        //First Version
        performDiamondSquare();
        //Adjusted OGLDEV
        performDiamondSquareOGLDEV(smoothness);
        performDiamondSquareV2();
    }

    
    private int calculateIndex(int xCoord, int yCoord) 
    {
        return (yCoord * (int)sideSize + xCoord);
    }

    //---------------
    //Self Written - Version 1
    //---------------
    #region
    
    private void performDiamondSquare() 
    {
        //Initial Corners
        
        float h1 = Random.Range(0, height);
        heightmap[calculateIndex(0, 0)] = h1;
        float h2 = Random.Range(0, height);
        heightmap[calculateIndex(maximumIndexCoordinate,0)] = h2;
        float h3 = Random.Range(0, height);
        heightmap[calculateIndex(0,maximumIndexCoordinate)] = h3;
        float h4 = Random.Range(0, height);
        heightmap[calculateIndex(maximumIndexCoordinate,maximumIndexCoordinate)] = h4;
        int iteration = 1;
        PerformDiamondStep(0, 0, maximumIndexCoordinate, maximumIndexCoordinate,iteration);
    }

    //Diamond Step
    private void PerformDiamondStep(int x1, int y1, int x2, int y2, int iteration)
    {
        float halfRectangleSize = (x2 - x1) / 2;
        if (halfRectangleSize < 1)
        {
            return;
        }
        int middleX = x1 + ((x2 - x1) / 2);
        int middleY = y1 + ((y2 - y1) / 2);
        heightmap[calculateIndex(middleX, middleY)] = DiamondAverage(x1, y1, x2, y2) + (Random.Range(-halfRectangleSize, halfRectangleSize) * Mathf.Pow(2, -smoothness));
        PerformSquareStep(middleX, middleY, x1, y1, x2, y2, iteration);
    }

    //Square Step
    private void PerformSquareStep(int middleX, int middleY, int x1, int y1, int x2, int y2, int iteration)
    {
        int dist = middleX - x1;
        //set heightmap with same X coordinate
        heightmap[calculateIndex(middleX, y1)] = SquareAverage(middleX, y1, dist) + (Random.Range(-dist, dist) * Mathf.Pow(2, -smoothness));
        heightmap[calculateIndex(middleX, y2)] = SquareAverage(middleX, y2, dist) + (Random.Range(-dist, dist) * Mathf.Pow(2, -smoothness));
        //set heightmap with same Y coordinate
        heightmap[calculateIndex(x1, middleY)] = SquareAverage(x1, middleY, dist) + (Random.Range(-dist, dist) * Mathf.Pow(2, -smoothness));
        heightmap[calculateIndex(x2, middleY)] = SquareAverage(x2, middleY, dist) + (Random.Range(-dist, dist) * Mathf.Pow(2, -smoothness));

        iteration++;
        PerformDiamondStep(x1, y1, middleX, middleY, iteration);
        PerformDiamondStep(middleX, middleY, x2, y2, iteration);
        PerformDiamondStep(x1, middleY, middleX, y2, iteration);
        PerformDiamondStep(middleX, y1, x2, middleY, iteration);
    }
    private float DiamondAverage(int x1, int y1, int x2, int y2)
    {
        float averageValue = 0;
        averageValue += heightmap[calculateIndex(x1, y1)];
        averageValue += heightmap[calculateIndex(x1, y2)];
        averageValue += heightmap[calculateIndex(x2, y1)];
        averageValue += heightmap[calculateIndex(x2, y2)];
        averageValue /= 4f;
        return averageValue;
    }

    private float SquareAverage(int x, int y, int dist)
    {
        float averageValue = 0;
        float valueCounter = 0;
        if (x + dist <= maximumIndexCoordinate)
        {
            averageValue += heightmap[calculateIndex(x + dist, y)];
            valueCounter++;
        }
        if (x - dist >= 0)
        {
            averageValue += heightmap[calculateIndex(x - dist, y)];
            valueCounter++;
        }
        if (y - dist >= 0)
        {
            averageValue += heightmap[calculateIndex(x, y - dist)];
            valueCounter++;
        }
        if (y + dist <= maximumIndexCoordinate)
        {
            averageValue += heightmap[calculateIndex(x, y + dist)];
            valueCounter++;
        }
        averageValue /= valueCounter;
        return averageValue;
    }
    #endregion
    //---------------
    //Adjusted OGDEV
    //---------------
    #region
    private void performDiamondSquareOGLDEV(float Roughness)
    {
        int rectSize = (int)mesh.bounds.size.x;
        float currentHeight = rectSize / 2.0f;
        float heightReduce = Mathf.Pow(2.0f, -Roughness);

        //Set Corner Values
        for (int z = 0; z <= mesh.bounds.size.z; z += rectSize)
        {
            for (int x = 0; x <= mesh.bounds.size.x; x += rectSize)
            {
                //heightmap[calculateIndex(x, z)].y = Random.Range(-currentHeight, currentHeight);
            }
        }
        int i = 0;
        while (rectSize > 1)
        {
            i++;
            //DiamondStep
            DiamondStep(rectSize, currentHeight);
            //SquareStep
            SquareStep(rectSize, heightReduce);
            rectSize /= 2;
            currentHeight *= heightReduce;
            //rectSize = 0;
        }
    }

    private void DiamondStep(int RectSize, float CurHeight)
    {
        int halfRectSize = RectSize / 2;
        for (int z = 0; z < mesh.bounds.size.z; z += RectSize)
        {
            for (int x = 0; x < mesh.bounds.size.x; x += RectSize)
            {
                int next_x = (x + RectSize) % (int)mesh.bounds.size.x;
                int next_z = (z + RectSize) % (int)mesh.bounds.size.z;

                if (next_x < x)
                {
                    next_x = (int)mesh.bounds.size.x - 1;
                }

                if (next_z < z)
                {
                    next_z = (int)mesh.bounds.size.z - 1;
                }

                float topLeft = heightmap[calculateIndex(x, z)];
                float topRight = heightmap[calculateIndex(next_x, z)];
                float bottomLeft = heightmap[calculateIndex(x, next_z)];
                float bottomRight = heightmap[calculateIndex(next_x, next_z)];

                int mid_x = x + halfRectSize;
                int mid_z = z + halfRectSize;

                float randValue = Random.Range(CurHeight, -CurHeight);
                float midPoint = (topLeft + topRight + bottomLeft + bottomRight) / 4.0f;
                heightmap[calculateIndex(mid_x, mid_z)] = midPoint + randValue;
            }
        }
    }

    private void SquareStep(int RectSize, float CurHeight)
    {
        int halfRectSize = RectSize / 2;

        for (int z = 0; z < mesh.bounds.size.z; z += RectSize)
        {
            for (int x = 0; x < mesh.bounds.size.x; x += RectSize)
            {
                int next_x = (x + RectSize) % (int)mesh.bounds.size.x;
                int next_z = (z + RectSize) % (int)mesh.bounds.size.z;

                int mid_x = x + halfRectSize;
                int mid_z = z + halfRectSize;

                int prev_mid_x = (x - halfRectSize + (int)mesh.bounds.size.x) % (int)mesh.bounds.size.x;
                int prev_mid_z = (z - halfRectSize + (int)mesh.bounds.size.z) % (int)mesh.bounds.size.z;

                float CurTopLeft = heightmap[calculateIndex(x, z)];
                float CurTopRight = heightmap[calculateIndex(next_x, z)];
                float CurCenter = heightmap[calculateIndex(mid_x, mid_z)];
                float prevZCenter = heightmap[calculateIndex(mid_x, prev_mid_z)];
                float curBotLeft = heightmap[calculateIndex(x, next_z)];
                float prevXCenter = heightmap[calculateIndex(prev_mid_x, mid_z)];

                float CurLeftMid = (CurTopLeft + CurCenter + curBotLeft + prevXCenter) / 4.0f + Random.Range(-CurHeight, CurHeight);
                float CurTopMid = (CurTopLeft + CurCenter + CurTopRight + prevZCenter) / 4.0f + Random.Range(-CurHeight, CurHeight);

                heightmap[calculateIndex(mid_x, z)] = CurTopMid;
                heightmap[calculateIndex(x, mid_z)] = CurLeftMid;

                if (x == 0)
                {
                    float nextHoriz = heightmap[calculateIndex(((int)mesh.bounds.size.x + halfRectSize) % (int)mesh.bounds.size.x, z)];
                    float prevHoriz = heightmap[calculateIndex(((int)mesh.bounds.size.x - halfRectSize) % (int)mesh.bounds.size.x, z)];
                    float nextVert = heightmap[calculateIndex((int)mesh.bounds.size.x, (mid_z + halfRectSize) % (int)mesh.bounds.size.z)];
                    float prevVert = heightmap[calculateIndex((int)mesh.bounds.size.x, (mid_z - halfRectSize) % (int)mesh.bounds.size.z)];
                    float borderAverage = (nextHoriz + prevHoriz + nextVert + prevVert) / 4.0f + Random.Range(-CurHeight, CurHeight);
                    heightmap[calculateIndex((int)mesh.bounds.size.x, mid_z)] = borderAverage;
                }
                if (z == 0)
                {
                    float nextHoriz = heightmap[calculateIndex((mid_x + halfRectSize) % (int)mesh.bounds.size.x, z)];
                    float prevHoriz = heightmap[calculateIndex((mid_x - halfRectSize) % (int)mesh.bounds.size.x, z)];
                    float nextVert = heightmap[calculateIndex((int)mesh.bounds.size.x, ((int)mesh.bounds.size.z + halfRectSize) % (int)mesh.bounds.size.z)];
                    float prevVert = heightmap[calculateIndex((int)mesh.bounds.size.x, ((int)mesh.bounds.size.z - halfRectSize) % (int)mesh.bounds.size.z)];
                    float borderAverage = (nextHoriz + prevHoriz + nextVert + prevVert) / 4.0f + Random.Range(-CurHeight, CurHeight);
                    heightmap[calculateIndex(mid_x, (int)mesh.bounds.size.z)] = borderAverage + Random.Range(-CurHeight, CurHeight);
                }
            }
        }
    }
    #endregion
    //---------------
    //Self Written - Version 2
    //---------------
    #region

    public void performDiamondSquareV2()
    {
        //Initial Corners

        float h1 = Random.Range(-height, height);
       //heightmap[calculateIndex(0, 0)] = h1;
        float h2 = Random.Range(-height, height);
        //heightmap[calculateIndex(maximumIndexCoordinate, 0)] = h2;
        float h3 = Random.Range(-height, height);
       // heightmap[calculateIndex(0, maximumIndexCoordinate)] = h3;
        float h4 = Random.Range(-height, height);
        //heightmap[calculateIndex(maximumIndexCoordinate, maximumIndexCoordinate)] = h4;
        float nextHeight = height * Mathf.Pow(2, -smoothness);
        PerformDiamondStepV2((int)sideSize,nextHeight);
    }

    //Diamond Step
    private void PerformDiamondStepV2(int rectangleSize, float height)
    {
        float halfRectangleSize = rectangleSize / 2;
        if (halfRectangleSize < 1)
        {
            return;
        }
        

        for (int x = 0; x < maximumIndexCoordinate; x += rectangleSize)
        {
            for (int y = 0; y < maximumIndexCoordinate; y += rectangleSize)
            {
                int nextX = (x + rectangleSize)%(int)sideSize;
                int nextY = (y + rectangleSize)%(int)sideSize;

                int middleX = x + (int)halfRectangleSize;
                int middleY = y + (int)halfRectangleSize;

                float topLeft = heightmap[calculateIndex(x, y)];
                float topRight = heightmap[calculateIndex(nextX,y)];
                float bottomLeft = heightmap[calculateIndex(x, nextY)];
                float bottomRight = heightmap[calculateIndex(nextX, nextY)];
                heightmap[calculateIndex(middleX, middleY)] = (topLeft + topRight + bottomLeft + bottomRight)/4.0f + (Random.Range(-height, height) );
            }

        }
        PerformSquareStepV2(rectangleSize, height);
    }

    //Square Step
    private void PerformSquareStepV2(int rectangleSize, float height)
    {
        float halfRectangleSize = rectangleSize / 2;

        for (int x = 0; x < maximumIndexCoordinate; x += rectangleSize)
        {
            for (int y = 0; y < maximumIndexCoordinate; y += rectangleSize)
            {
                int middleX = x + (int)halfRectangleSize;
                int middleY = y + (int)halfRectangleSize;

                //coordinates of target points
                int lowerY = (middleY - (int)halfRectangleSize) % (int)sideSize;
                int upperY = (middleY + (int)halfRectangleSize) % (int)sideSize;
                int lowerX = (middleX - (int)halfRectangleSize) % (int)sideSize;
                int upperX = (middleX + (int)halfRectangleSize) % (int)sideSize;

                //LowerY
                float neighbour1 = heightmap[calculateIndex(middleX, (lowerY + (int)halfRectangleSize + (int)sideSize) % (int)sideSize)];
                float neighbour2 = heightmap[calculateIndex(middleX, (lowerY - (int)halfRectangleSize + (int)sideSize) % (int)sideSize)];
                float neighbour3 = heightmap[calculateIndex((middleX - (int)halfRectangleSize + (int)sideSize) % (int)sideSize, lowerY)];
                float neighbour4 = heightmap[calculateIndex((middleX + (int)halfRectangleSize + (int)sideSize) % (int)sideSize, lowerY)];
                heightmap[calculateIndex(middleX,lowerY)] = (neighbour1+neighbour2 + neighbour3 + neighbour4) / 4.0f + (Random.Range(-height,height));

                //UpperY
                neighbour1 = heightmap[calculateIndex(middleX, (upperY + (int)halfRectangleSize + (int)sideSize) % (int)sideSize)];
                neighbour2 = heightmap[calculateIndex(middleX, (upperY - (int)halfRectangleSize + (int)sideSize) % (int)sideSize)];
                neighbour3 = heightmap[calculateIndex((middleX - (int)halfRectangleSize + (int)sideSize) % (int)sideSize, upperY)];
                neighbour4 = heightmap[calculateIndex((middleX + (int)halfRectangleSize + (int)sideSize) % (int)sideSize, upperY)];
                heightmap[calculateIndex(middleX, upperY)] = (neighbour1 + neighbour2 + neighbour3 + neighbour4) / 4.0f + (Random.Range(-height, height));

                //LowerX
                neighbour1 = heightmap[calculateIndex(lowerX, (middleY + (int)halfRectangleSize + (int)sideSize) % (int)sideSize)];
                neighbour2 = heightmap[calculateIndex(lowerX, (middleY - (int)halfRectangleSize + (int)sideSize) % (int)sideSize)];
                neighbour3 = heightmap[calculateIndex((lowerX - (int)halfRectangleSize + (int)sideSize) % (int)sideSize, middleY)];
                neighbour4 = heightmap[calculateIndex((lowerX + (int)halfRectangleSize + (int)sideSize) % (int)sideSize, middleY)];
                heightmap[calculateIndex(lowerX, middleY)] = (neighbour1 + neighbour2 + neighbour3 + neighbour4) / 4.0f + (Random.Range(-height,height));

                //UpperX
                neighbour1 = heightmap[calculateIndex(upperX, (middleY + (int)halfRectangleSize + (int)sideSize) % (int)sideSize)];
                neighbour2 = heightmap[calculateIndex(upperX, (middleY - (int)halfRectangleSize + (int)sideSize) % (int)sideSize)];
                neighbour3 = heightmap[calculateIndex((upperX - (int)halfRectangleSize + (int)sideSize) % (int)sideSize, middleY)];
                neighbour4 = heightmap[calculateIndex((upperX + (int)halfRectangleSize + (int)sideSize) % (int)sideSize, middleY)];
                heightmap[calculateIndex(upperX, middleY)] = (neighbour1 + neighbour2 + neighbour3 + neighbour4) / 4.0f + (Random.Range(-height,height));
            }
        }

        rectangleSize /= 2;
        height *= Mathf.Pow(2, -smoothness);
        PerformDiamondStepV2(rectangleSize,height);
    }

    #endregion
    
}
