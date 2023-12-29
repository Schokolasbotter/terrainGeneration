using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class meshControl : MonoBehaviour
{
    private FFT fft;
    private diamondSquare ds;
    private Mesh mesh;
    public enum meshChoice { FourrierTransform,DiamondSquare,AverageMix,AddMix };

    public meshChoice choice = meshChoice.FourrierTransform;

    // Start is called before the first frame update
    void Start()
    {
        fft = GetComponent<FFT>();
        ds = GetComponent<diamondSquare>();
        mesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] vertices = mesh.vertices;
        switch(choice)
        {
            case meshChoice.FourrierTransform:
                for(int i  = 0; i < vertices.Length; i++)
                {
                    //vertices[i].y = fft.heightmap[i];
                }
                break;
            case meshChoice.DiamondSquare:
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].y = ds.heightmap[i];
                }
                break;
            case meshChoice.AverageMix:
                for (int i = 0; i < vertices.Length; i++)
                {
                    //vertices[i].y = (diamondSquare.heightmap[i]+ fft.heightmap[i])/2;
                }
                break;
            case meshChoice.AddMix:
                for (int i = 0; i < vertices.Length; i++)
                {
                    //vertices[i].y = diamondSquare.heightmap[i]+ fft.heightmap[i];
                }
                break;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
