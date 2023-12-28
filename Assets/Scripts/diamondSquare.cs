using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class diamondSquare : MonoBehaviour
{
    public float height = 5f;
    [Range(0f, 2f)] public float smoothness;
    private float sideSize;
    Mesh mesh;
    Vector3[] vertices;
    int maximumIndexCoordinate = 0;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        sideSize = Mathf.Sqrt(mesh.vertices.Length);
        maximumIndexCoordinate = (int)sideSize - 1;
        vertices = mesh.vertices;

        Debug.Log(mesh.bounds.size);
        performDiamondSquare2(smoothness);

        //Rewrite Vertices
        mesh.vertices = vertices; //Overwrite Vertices
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private int calculateIndex(int xCoord, int yCoord) 
    {
        return (yCoord * (int)sideSize + xCoord);
    }

    private float DiamondAverage(int x1,int y1, int x2, int y2)
    {
        float averageValue = 0;
        averageValue += vertices[calculateIndex(x1, y1)].y;
        averageValue += vertices[calculateIndex(x1, y2)].y;
        averageValue += vertices[calculateIndex(x2, y1)].y;
        averageValue += vertices[calculateIndex(x2, y2)].y;
        averageValue /= 4f;
        return averageValue;
    }

    private float SquareAverage(int x, int y, int dist) 
    {
        float averageValue = 0;
        float valueCounter = 0;
        if (x + dist <= maximumIndexCoordinate) {
            averageValue += vertices[calculateIndex(x + dist, y)].y;
            valueCounter++;
        }
        if(x- dist >= 0)
        {
            averageValue += vertices[calculateIndex(x - dist, y)].y;
            valueCounter++;
        }
        if (y - dist >= 0)
        {
            averageValue += vertices[calculateIndex(x, y - dist)].y;
            valueCounter++;
        }
        if (y + dist <= maximumIndexCoordinate)
        {
            averageValue += vertices[calculateIndex(x, y + dist)].y;
            valueCounter++;
        }   
        averageValue /= valueCounter;
        return averageValue;
    }


    private void performDiamondSquare() 
    {
        //Initial Corners
        
        float h1 = Random.Range(0, height);
        vertices[calculateIndex(0, 0)].y = h1;
        float h2 = Random.Range(0, height);
        vertices[calculateIndex(maximumIndexCoordinate,0)].y = h2;
        float h3 = Random.Range(0, height);
        vertices[calculateIndex(0,maximumIndexCoordinate)].y = h3;
        float h4 = Random.Range(0, height);
        vertices[calculateIndex(maximumIndexCoordinate,maximumIndexCoordinate)].y = h4;
        int iteration = 1;
        PerformDiamondStep(0, 0, maximumIndexCoordinate, maximumIndexCoordinate,iteration);
    }
    private void performDiamondSquare2(float Roughness) 
    { 
        int rectSize = (int)mesh.bounds.size.x;
        float currentHeight = rectSize / 2.0f;
        float heightReduce = Mathf.Pow(2.0f, -Roughness);

        //Set Corner Values
        for (int z = 0; z <= mesh.bounds.size.z; z += rectSize)
        {
            for (int x = 0; x <= mesh.bounds.size.x; x += rectSize)
            {
                //Debug.Log("St starting Value at X: " + x + " Z: " + z);
                //vertices[calculateIndex(x, z)].y = Random.Range(-currentHeight, currentHeight);
            }
        }
                int i = 0;
        while (rectSize > 1)
        {
            Debug.Log("Iteration : " + i);
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
        for(int z = 0; z < mesh.bounds.size.z; z+= RectSize){
            for(int x =0; x < mesh.bounds.size.x; x+= RectSize)
            {
                int next_x = (x+RectSize)%(int)mesh.bounds.size.x;
                int next_z = (z+RectSize)%(int)mesh.bounds.size.z;

                if(next_x < x)
                {
                    next_x = (int)mesh.bounds.size.x - 1;
                }

                if (next_z < z)
                {
                    next_z = (int)mesh.bounds.size.z - 1;
                }

                float topLeft = vertices[calculateIndex(x, z)].y;
                float topRight = vertices[calculateIndex(next_x, z)].y;
                float bottomLeft = vertices[calculateIndex(x, next_z)].y;
                float bottomRight = vertices[calculateIndex(next_x, next_z)].y;

                int mid_x = x + halfRectSize;
                int mid_z = z + halfRectSize;

                float randValue = Random.Range(CurHeight, -CurHeight);
                float midPoint = (topLeft + topRight + bottomLeft + bottomRight) / 4.0f;
                Debug.Log("Change MidPoint at X: " + mid_x + " Z: " + mid_z);
                vertices[calculateIndex(mid_x, mid_z)].y = midPoint + randValue;
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
                Debug.Log("Z: " + z + " X: " + x);
                int next_x = (x + RectSize) % (int)mesh.bounds.size.x;
                int next_z = (z + RectSize) % (int)mesh.bounds.size.z;

                int mid_x = x + halfRectSize;
                int mid_z = z + halfRectSize;

                int prev_mid_x = (x-halfRectSize + (int)mesh.bounds.size.x) % (int)mesh.bounds.size.x;
                int prev_mid_z = (z-halfRectSize + (int)mesh.bounds.size.z) % (int)mesh.bounds.size.z;

                float CurTopLeft = vertices[calculateIndex(x, z)].y;
                float CurTopRight = vertices[calculateIndex(next_x, z)].y;
                float CurCenter = vertices[calculateIndex(mid_x,mid_z)].y;
                float prevZCenter = vertices[calculateIndex(mid_x, prev_mid_z)].y;
                float curBotLeft = vertices[calculateIndex(x, next_z)].y;
                float prevXCenter = vertices[calculateIndex(prev_mid_x, mid_z)].y;

                float CurLeftMid = (CurTopLeft + CurCenter + curBotLeft + prevXCenter) / 4.0f + Random.Range(-CurHeight, CurHeight);
                float CurTopMid = (CurTopLeft + CurCenter + CurTopRight + prevZCenter) / 4.0f + Random.Range(-CurHeight, CurHeight);

                Debug.Log("Change Vertex at X: " + mid_x + " Z: " + z);
                vertices[calculateIndex(mid_x,z)].y = CurTopMid;
                Debug.Log("Change Vertex at X: " + x + " Z: " + mid_z);
                vertices[calculateIndex(x,mid_z)].y = CurLeftMid;
                
                if(x == 0)
                {
                    float nextHoriz = vertices[calculateIndex(((int)mesh.bounds.size.x + halfRectSize)% (int)mesh.bounds.size.x, z)].y;
                    float prevHoriz = vertices[calculateIndex(((int)mesh.bounds.size.x - halfRectSize)% (int)mesh.bounds.size.x, z)].y;
                    float nextVert =  vertices[calculateIndex((int)mesh.bounds.size.x, (mid_z + halfRectSize) % (int)mesh.bounds.size.z)].y;
                    float prevVert =  vertices[calculateIndex((int)mesh.bounds.size.x, (mid_z - halfRectSize) % (int)mesh.bounds.size.z)].y;
                    float borderAverage =  (nextHoriz + prevHoriz + nextVert + prevVert) / 4.0f + Random.Range(-CurHeight, CurHeight);                                            
                    Debug.Log("Change Vertex at X: " + (int)mesh.bounds.size.x + " Z: " + mid_z);
                    vertices[calculateIndex((int)mesh.bounds.size.x, mid_z)].y = borderAverage;
                }
                if(z == 0)
                {
                    float nextHoriz = vertices[calculateIndex((mid_x + halfRectSize) % (int)mesh.bounds.size.x, z)].y;
                    float prevHoriz = vertices[calculateIndex((mid_x - halfRectSize) % (int)mesh.bounds.size.x, z)].y;
                    float nextVert = vertices[calculateIndex((int)mesh.bounds.size.x, ((int)mesh.bounds.size.z + halfRectSize) % (int)mesh.bounds.size.z)].y;
                    float prevVert = vertices[calculateIndex((int)mesh.bounds.size.x, ((int)mesh.bounds.size.z - halfRectSize) % (int)mesh.bounds.size.z)].y;
                    float borderAverage = (nextHoriz + prevHoriz + nextVert + prevVert) / 4.0f + Random.Range(-CurHeight, CurHeight);
                    Debug.Log("Change Vertex at X: " + mid_x + " Z: " + (int)mesh.bounds.size.z);
                    vertices[calculateIndex(mid_x, (int)mesh.bounds.size.z)].y = borderAverage + Random.Range(-CurHeight, CurHeight);
                }
            }
        }
    }

    
    //Diamond Step
    private void PerformDiamondStep(int x1, int y1, int x2, int y2, int iteration)
    {
        float halfRectagleSize = (x2 - x1)/2;
        if(halfRectagleSize <1){
            return;
        }
        int middleX = x1 + ((x2 - x1) / 2);
        int middleY = y1 + ((y2 - y1) / 2);
        vertices[calculateIndex(middleX, middleY)].y = DiamondAverage(x1,y1,x2,y2) + (Random.Range(-halfRectagleSize,halfRectagleSize)*Mathf.Pow(2,-smoothness));
        PerformSquareStep(middleX, middleY,x1,y1,x2,y2, iteration);
    }

    //Square Step
    private void PerformSquareStep(int middleX, int middleY, int x1, int y1, int x2, int y2, int iteration)
    {
        int dist = middleX - x1;
        //set vertices with same X coordinate
        vertices[calculateIndex(middleX,y1)].y = SquareAverage(middleX, y1, dist) + (Random.Range(-dist, dist) * Mathf.Pow(2, -smoothness));
        vertices[calculateIndex(middleX,y2)].y = SquareAverage(middleX, y2, dist) + (Random.Range(-dist, dist) * Mathf.Pow(2, -smoothness));
        //set vertices with same Y coordinate
        vertices[calculateIndex(x1,middleY)].y = SquareAverage(x1, middleY, dist) + (Random.Range(-dist, dist) * Mathf.Pow(2, -smoothness));
        vertices[calculateIndex(x2,middleY)].y = SquareAverage(x2, middleY, dist) + (Random.Range(-dist, dist) * Mathf.Pow(2, -smoothness));

        iteration++;
        PerformDiamondStep(x1, y1, middleX, middleY, iteration);
        PerformDiamondStep(middleX, middleY, x2, y2, iteration);
        PerformDiamondStep(x1, middleY, middleX, y2, iteration);
        PerformDiamondStep(middleX, y1, x2, middleY, iteration);
    }
}
