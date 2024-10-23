using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject blade;
    public GameObject handle;

    public Weapon(GameObject blade, GameObject handle)
    {
        this.blade = blade;
        this.handle = handle;
    }

    // Start is called before the first frame update
    void Start() {
        connectToBlade();
    }

    // Update is called once per frame
    void Update() { }

    public void Swing()
    {
        Debug.Log("weapon swung");
    }

    public GameObject getBlade()
    {
        return blade;
    }

    public void connectToBlade()
    {
        // Get the connection points on the blade and handle
        Transform bladeConnectionPoint = blade.GetComponent<ExampleBlade>().handleConnectionPoint.transform;
        Transform handleConnectionPoint = handle.GetComponent<ExampleHandle>().handleConnectionPoint.transform;

        // Parent the blade to the handle first (so we can adjust relative positioning)
        blade.transform.SetParent(handle.transform);

        // Calculate the offset between the handle's connection point and the blade's connection point
        Vector3 positionOffset = handleConnectionPoint.position - bladeConnectionPoint.position;

        // Move the blade by the calculated offset to align connection points
        blade.transform.position += positionOffset;

        // Rotate the blade so the blade's connection point matches the handle's rotation
        blade.transform.rotation = handleConnectionPoint.rotation;

        // Optionally, you could log or debug information
        Debug.Log("Blade connected to handle.");
    }


    public void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(blade.GetComponent<IBlade>().DamageValue);
            Debug.Log("Player hit");
        }
    }
}
