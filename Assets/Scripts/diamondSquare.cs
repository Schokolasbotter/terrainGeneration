using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class diamondSquare : MonoBehaviour
{
    public int width, length;
    Mesh planeMesh;
    Vector3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        planeMesh = new Mesh();
        vertices = new Vector3[width * length];
        createPlaneMesh();

        //Set Corner Values
        setCornerValues();

        //Rewrite Vertices
        planeMesh.vertices = vertices;
        Debug.Log(planeMesh.vertexCount);
        planeMesh.RecalculateBounds();
        planeMesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = planeMesh;
    }

    //create the plane mesh
    private void createPlaneMesh()
    {
        //Go throug rows
        for(int i = 0; i < width; i++)
        {
            //Go through columns
            for(int j = 0; j < length; j++)
            {
                int vertexIndex = i * width + j;
                vertices[vertexIndex] = new Vector3(i,0f,j);
            }
        }
    }
    //Set Initial Corner Values
    private void setCornerValues()
    {
        vertices[1].y = 1f;
    } 
    //Diamond Step
    //Square Step

}
