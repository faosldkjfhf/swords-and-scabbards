using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPost : MonoBehaviour
{
    private Material material;

    void Awake()
    {
        material = new Material(Shader.Find("Hidden/FinalShader"));
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, material);
    }
}
