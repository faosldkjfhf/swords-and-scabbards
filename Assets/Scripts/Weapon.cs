using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject blade;
    public List<GameObject> bladePrefabs; // List of blade prefabs
    public GameObject handle;
    private bool isAttacking = false;
    private bool dealtDamage = false;
    public GameObject wielder;

    public Weapon(GameObject blade, GameObject handle, GameObject wielder)
    {
        this.blade = blade;
        this.handle = handle;
        this.wielder = wielder;
    }

    void Awake()
    {
        SelectAndCreateBlade();
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
        blade.transform.position = handle.transform.position; // Example: instantiate at origin

        Debug.Log("Blade created: " + blade.name);
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
        GameObject bladeHolder = new GameObject("BladeHolder");

        // Set the bladeHolder's position and rotation to match the blade's initial transform
        bladeHolder.transform.position = blade.transform.position;
        bladeHolder.transform.rotation = blade.transform.rotation;

        // Parent the blade to the bladeHolder to isolate its transformations
        blade.transform.SetParent(bladeHolder.transform);

        // Get the connection points on the blade and handle
        Transform bladeConnectionPoint = blade
            .GetComponent<ExampleBlade>()
            .handleConnectionPoint.transform;
        Transform handleConnectionPoint = handle
            .GetComponent<ExampleHandle>()
            .handleConnectionPoint.transform;

        // Store the current global position and rotation of the blade connection point
        Vector3 bladeGlobalPosition = bladeConnectionPoint.position;
        Quaternion bladeGlobalRotation = bladeConnectionPoint.rotation;

        // Calculate the global position and rotation offsets based on the connection points
        Vector3 positionOffset = handleConnectionPoint.position - bladeGlobalPosition;
        Quaternion rotationOffset =
            handleConnectionPoint.rotation * Quaternion.Inverse(bladeGlobalRotation);

        // Parent the bladeHolder to the handle (this is the final step)
        bladeHolder.transform.SetParent(handle.transform);

        // Apply the previously calculated position offset to the bladeHolder
        bladeHolder.transform.position += positionOffset;

        // Apply the rotation offset
        bladeHolder.transform.rotation = rotationOffset * bladeHolder.transform.rotation;

        // Optionally, you could log or debug information
        Debug.Log("Blade connected to handle through BladeHolder.");
    }

    public void OnCollisionEnter(Collision other)
    {
        if (isAttacking && !dealtDamage)
        {
            if (other.gameObject.CompareTag("Player") && other.gameObject != wielder)
            {
                Debug.Log("Player hit");
                dealtDamage = true;
                other.gameObject
                    .GetComponent<PlayerController>()
                    .TakeDamage(blade.GetComponent<IBlade>().DamageValue);
            }
        }
    }
}
