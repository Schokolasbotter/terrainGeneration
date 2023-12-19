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
    [Range(0f, 1f)] public float smoothness;
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

        performDiamondSquare();

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

    
    //Diamond Step
    private void PerformDiamondStep(int x1, int y1, int x2, int y2, int iteration)
    {
        Debug.Log(iteration);
        if((x2 - x1)/ 2 <1){
            return;
        }
        int middleX = x1 + ((x2 - x1) / 2);
        int middleY = y1 + ((y2 - y1) / 2);
        vertices[calculateIndex(middleX, middleY)].y = DiamondAverage(x1,y1,x2,y2) + (Random.Range(0f,1f)*Mathf.Pow(2,-smoothness));
        PerformSquareStep(middleX, middleY,x1,y1,x2,y2, iteration);
    }

    //Square Step
    private void PerformSquareStep(int middleX, int middleY, int x1, int y1, int x2, int y2, int iteration)
    {
        int dist = middleX - x1;
        //set vertices with same X coordinate
        vertices[calculateIndex(middleX,y1)].y = SquareAverage(middleX, y1, dist) + (Random.Range(0f, 1f) * Mathf.Pow(2, -smoothness));
        vertices[calculateIndex(middleX,y2)].y = SquareAverage(middleX, y2, dist) + (Random.Range(0f, 1f) * Mathf.Pow(2, -smoothness));
        //set vertices with same Y coordinate
        vertices[calculateIndex(x1,middleY)].y = SquareAverage(x1, middleY, dist) + (Random.Range(0f, 1f) * Mathf.Pow(2, -smoothness));
        vertices[calculateIndex(x2,middleY)].y = SquareAverage(x2, middleY, dist) + (Random.Range(0f, 1f) * Mathf.Pow(2, -smoothness));

        iteration++;
        PerformDiamondStep(x1, y1, middleX, middleY, iteration);
        PerformDiamondStep(middleX, middleY, x2, y2, iteration);
        PerformDiamondStep(x1, middleY, middleX, y2, iteration);
        PerformDiamondStep(middleX, y1, x2, middleY, iteration);
    }
}
