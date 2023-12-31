using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIMAnager : MonoBehaviour
{
    public TMP_Text MeshSize;
    public TMP_Text HeightSize;
    public TMP_Text SmoothnessSize;
    public TMP_Text FilterVal;
    public Slider MeshSizeSlider;
    public Slider HeightSlider;
    public Slider SmoothnessSlider;
    public Slider FilterSlider;

    public TMP_Text GenerateText;

    public GenerateMesh generateMesh;
    public meshControl meshControl;
    

    public meshControl.meshChoice currentChoice = meshControl.meshChoice.DiamondSquare;

    [SerializeField] private FFT fftScript;
    [SerializeField] private diamondSquare dsScript; 
    [SerializeField] private Shader shaderScript;
    // Start is called before the first frame update

    private void Start()
    {
        int n = (int)Mathf.Pow(2f, generateMesh.N) + 1;
        MeshSize.text = "Mesh Size: " + n.ToString() + "x" + n.ToString();

        SmoothnessSize.text = "Smoothness: " + dsScript.smoothness.ToString();
        SmoothnessSlider.value = dsScript.smoothness;
        FilterVal.text = "Filter: " + fftScript.filter.ToString();
        FilterSlider.value = fftScript.filter;
        HeightSize.text = "Height: " + "30";
    }
    public void UpdateMeshSize()
    {
        int n = (int)Mathf.Pow(2f, MeshSizeSlider.value) + 1;
        MeshSize.text = "Mesh Size: " + n.ToString() + "x" + n.ToString();
        generateMesh.N = (int)MeshSizeSlider.value;
    }

    public void UpdateHeight()
    {
        float DSHeight = HeightSlider.value;
        HeightSize.text = "Height Multiplier: " + DSHeight.ToString();
        dsScript.height = DSHeight;
        fftScript.heightMult = DSHeight;
        shaderScript.UpdateHeight(DSHeight);
    }

    public void UpdateSmoothness()
    {
        float DSSmoothness = SmoothnessSlider.value;
        DSSmoothness /= 2;
        SmoothnessSize.text = "Smoothness: " + DSSmoothness.ToString();

        dsScript.smoothness = DSSmoothness;
    }

    public void UpdateFilter()
    {
        float Filtern = FilterSlider.value;
        FilterVal.text = "Filter: " + Filtern.ToString();
        fftScript.filter = Filtern;
    }
    public void RegenNoise()
    {
        fftScript.regenerateNoiseScale();
        shaderScript.UpdateNoise();
    }

    public void setDS()
    {
        currentChoice = meshControl.meshChoice.DiamondSquare;
        GenerateText.text = "DiamondSquare";
        setChoice();
    }

    public void setFourier()
    {
        currentChoice = meshControl.meshChoice.FourrierTransform;
        GenerateText.text = "Fourier";
        setChoice();
    }
    public void setShader()
    {
        currentChoice = meshControl.meshChoice.Shader;
        GenerateText.text = "Shader";
        setChoice();
    }

    public void setAdd()
    {
        currentChoice = meshControl.meshChoice.AddMix;
        GenerateText.text = "Add Mix";
        setChoice();
    }
    public void setAverage()
    {
        currentChoice = meshControl.meshChoice.AverageMix;
        GenerateText.text = "Average Mix";
        setChoice();
    }
    private void setChoice()
    {
        meshControl.choice = currentChoice;
        meshControl.applySetting();
    }

    public void GenerateNewSet()
    {
        switch (meshControl.choice)
        {
            case meshControl.meshChoice.DiamondSquare:
                dsScript.performDiamondSquareV2();
                break;
            case meshControl.meshChoice.FourrierTransform:
                fftScript.RebuildHeightMap();
                break;

        }
    }
}
