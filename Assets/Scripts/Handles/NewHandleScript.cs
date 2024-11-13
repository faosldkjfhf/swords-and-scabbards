using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHandleScript : MonoBehaviour
{


    public GameObject blade;
    public List<GameObject> bladePrefabs; // List of blade prefabs
    public GameObject wielder;
    public RuntimeAnimatorController animationStyle;

    [SerializeField]
    private Transform weaponPlacement;
    [SerializeField]
    private Transform rightHandPlacement;
    [SerializeField]
    private Transform leftHandPlacement;
    // Start is called before the first frame update


    // Start is called before the first frame update
    void Awake()
    {
        SelectAndCreateBlade();
        CreateParentWithChildren(gameObject, blade);

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

        Transform bladeConnectionPoint = blade.GetComponent<ExampleBlade>().handleConnectionPoint.transform;
        Transform handleConnectionPoint = getHGandleConnectionPoint("handleConnection");


        // Match the position and rotation of the blade to the handle's connection point
        blade.transform.position = handleConnectionPoint.position;
        blade.transform.rotation = handleConnectionPoint.rotation;

        // Now calculate the relative offset from the blade connection point to properly adjust position and rotation
        Vector3 connectionOffset = bladeConnectionPoint.position - blade.transform.position;

        // Move the blade so that the connection points align properly
        blade.transform.position -= connectionOffset;

        Debug.Log("Blade created: " + blade.name);
    }

    public Transform getHGandleConnectionPoint(string childName)
    {
        // Loop through all child transforms
        foreach (Transform child in transform)
        {
            if (child.name == childName)
            {
                return child;
            }
        }

        // If no child is found with the given name, return null
        return null;
    }



    public void CreateParentWithChildren(GameObject child1, GameObject child2)
    {
        // Create a new GameObject with the specified name
        GameObject newParent = new GameObject(gameObject.name + " with " + blade.name);
        newParent.AddComponent<NewEmptyWeapon>();
        //wielder = newParent.GetComponent<NewEmptyWeapon>().wielder;
        //newParent.transform.position = wielder.GetComponent<BetterPlayerController>().weaponPoint.transform.position;
        //newParent.transform.rotation = wielder.GetComponent<BetterPlayerController>().weaponPoint.transform.rotation;

        // Parent the first GameObject, if provided
        if (child1 != null)
        {
            child1.transform.SetParent(newParent.transform);
        }

        // Parent the second GameObject, if provided
        if (child2 != null)
        {
            child2.transform.SetParent(newParent.transform);
        }
        newParent.GetComponent<NewEmptyWeapon>().blade = blade;
        newParent.GetComponent<NewEmptyWeapon>().handle = gameObject;
        newParent.GetComponent<NewEmptyWeapon>().animationStyle = animationStyle;
        
    }

}
