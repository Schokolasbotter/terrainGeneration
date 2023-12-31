using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shader : MonoBehaviour
{
    public Material baseMaterial;
    public Material terrainMaterial;

    private Renderer _renderer;

    // Start is called before the first frame update
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
    public void UpdateHeight(float height)
    {
        terrainMaterial.SetFloat("_MapHeight", height);
    }
    public void UpdateNoise()
    {
        terrainMaterial.SetFloat("_NoiseScale", Random.Range(0, 100));
    }

    private void OnEnable()
    {
        _renderer.material = terrainMaterial;
    }
    private void OnDisable()
    {
        _renderer.material = baseMaterial;
    }
}
