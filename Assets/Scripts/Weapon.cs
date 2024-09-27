using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private IBlade blade;
    private IHandle handle;

    public Weapon(IBlade blade, IHandle handle)
    {
        this.blade = blade;
        this.handle = handle;
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void Swing()
    {
        Debug.Log("weapon swung");
    }

    public IBlade getBlade()
    {
        return blade;
    }
}
