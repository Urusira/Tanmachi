using System;
using UnityEngine;

[ExecuteInEditMode]
public class GetMainLightDirection : MonoBehaviour
{
    private static readonly int MainLightDirection = Shader.PropertyToID("_MainLightDirection");
    [SerializeField] private Material skyboxMaterial;

    private void Update()
    {
        skyboxMaterial.SetVector(MainLightDirection, transform.forward);
    }
}
