using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyWeapon : MonoBehaviour
{
    public GameObject blade;
    public List<GameObject> bladePrefabs; // List of blade prefabs
    public List<GameObject> handlePrefabs; // List of blade prefabs
    public GameObject handle;
    private bool isAttacking = false;
    private bool dealtDamage = false;
    public GameObject wielder;
    public RuntimeAnimatorController animationStyle;



    // Start is called before the first frame update
    void Awake()
    {
        SelectAndCreateBlade();
        SelectAndCreateHandle();
    }
    // Start is called before the first frame update
    void Start()
    {

        connectToBlade();
    }

    // Update is called once per frame
    void Update() { }

    public void Swing()
    {
        Debug.Log("weapon swung");
    }

    public void setAttacking(bool attacking)
    {
        if (attacking == false)
        {
            dealtDamage = false;
        }

        isAttacking = attacking;
    }

    public void SelectAndCreateBlade()
    {
        if (bladePrefabs.Count == 0)
        {
            Debug.LogError("No blade prefabs assigned to the weapon.");
            return;
        }

        // Randomly select a blade prefab
        int randomIndex = Random.Range(0, bladePrefabs.Count);

        // Instantiate the selected blade prefab in the scene
        blade = Instantiate(bladePrefabs[randomIndex]);

        // Optionally, you can set the blade's position here if needed
        blade.transform.position = transform.position;
        blade.transform.rotation = transform.rotation; // Example: instantiate at origin

        Debug.Log("Blade created: " + blade.name);
    }

    public void SelectAndCreateHandle()
    {
        if (handlePrefabs.Count == 0)
        {
            Debug.LogError("No handle prefabs assigned to the weapon.");
            return;
        }

        // Randomly select a blade prefab
        int randomIndex = Random.Range(0, handlePrefabs.Count);

        // Instantiate the selected blade prefab in the scene
        handle = Instantiate(handlePrefabs[randomIndex]);

        // Optionally, you can set the blade's position here if needed
        handle.transform.position = transform.position;
        handle.transform.rotation = transform.rotation;// Example: instantiate at origin
        animationStyle = handle.GetComponent<ExampleHandle>().animationStyle;

        Debug.Log("handle created: " + handle.name);
    }

    public GameObject getBlade()
    {
        return blade;
    }

    public void connectToBlade()
    {
        if (blade == null || handle == null)
        {
            Debug.LogError("Blade or handle is missing.");
            return;
        }

        // Create an empty GameObject as a holder to prevent deformation during rotation
        //GameObject weaponHolder = new GameObject("weaponHolder");

        // Set the bladeHolder's position and rotation to match the blade's initial transform
        transform.position = handle.transform.position;
        transform.rotation = handle.transform.rotation;


        // Get the connection points on the blade and handle
        Transform bladeConnectionPoint = blade.GetComponent<ExampleBlade>().handleConnectionPoint.transform;
        Transform handleConnectionPoint = handle.GetComponent<ExampleHandle>().handleConnectionPoint.transform;

        // Match the position and rotation of the blade to the handle's connection point
        blade.transform.position = handleConnectionPoint.position;
        blade.transform.rotation = handleConnectionPoint.rotation;

        // Now calculate the relative offset from the blade connection point to properly adjust position and rotation
        Vector3 connectionOffset = bladeConnectionPoint.position - blade.transform.position;

        // Move the blade so that the connection points align properly
        blade.transform.position -= connectionOffset;

        blade.transform.SetParent(transform);
        handle.transform.SetParent(transform);

        // Optionally, you could log or debug information
        Debug.Log("Blade connected to handle through BladeHolder.");
        gameObject.name = blade.name + handle.name;
    }

    public void OnCollisionEnter(Collision other)
    {
        if (isAttacking && !dealtDamage)
        {
            if (other.gameObject.CompareTag("Player") && other.gameObject != wielder)
            {
                Debug.Log("Player hit");
                dealtDamage = true;
                // other.gameObject
                //     .GetComponent<PlayerController>()
                //     .TakeDamage(blade.GetComponent<IBlade>().DamageValue);
            }
        }
    }
}
