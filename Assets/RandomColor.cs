using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    private SkinnedMeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<SkinnedMeshRenderer>();
        mesh.materials[0].SetColor("_Color", Random.ColorHSV());
    }

    // Update is called once per frame
    void Update() { }
}
