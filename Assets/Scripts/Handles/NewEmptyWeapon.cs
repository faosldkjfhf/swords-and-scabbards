using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEmptyWeapon : MonoBehaviour
{

    public GameObject blade;
    public GameObject handle;
    private bool isAttacking = false;
    private bool dealtDamage = false;
    public GameObject wielder;
    public RuntimeAnimatorController animationStyle;
    bool done = false;

    [SerializeField]
    private Transform weaponPoint;
    // Start is called before the first frame update
    void Start()
    {
        FindWeaponPlacement();
        transform.SetParent(weaponPoint,true);
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void setAttacking(bool attacking)
    {
        if (attacking == false)
        {
            dealtDamage = false;
        }

        isAttacking = attacking;
    }

    public void Swing()
    {
        Debug.Log("weapon swung");
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject != blade)
        {
            return; // Ignore collisions not involving the blade
        }

        // Check if the collided GameObject has the tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Exclude the GameObject named "Wielder"
            if (collision.gameObject != wielder)
            {
                Debug.LogError($"Collided with Player: {collision.gameObject.name}");
                // Add your logic here for handling the collision
                //FILL THIS IN FOR DAMAGE DIPSHIT
            }
            else
            {
                Debug.LogError("Collision ignored with Wielder and ignored the damage");
            }
        }
    }



    private void FindWeaponPlacement()
    {
        if (wielder == null)
        {
            Debug.LogError("Wielder is not assigned.");
            return;
        }


        // Recursively search for the child named "weapon placement"
        foreach (Transform child in wielder.GetComponentsInChildren<Transform>())
        {
            if (child.name == "WeaponPoint")
            {
                weaponPoint = child;
                break;
            }
        }

        if (weaponPoint != null)
        {
            Debug.Log("Weapon placement found: " + weaponPoint.name);
        }
        else
        {
            Debug.LogError("Weapon placement not found in wielder's children.");
        }
    }

    private void positionToHand()
    {
        GameObject weaponPointObject = FindNestedGameObject(wielder, "WeaponPoint");
        transform.position = weaponPointObject.transform.position;
        transform.rotation = weaponPointObject.transform.rotation;
        

    }


    private GameObject FindNestedGameObject(GameObject parent, string name)
    {
        // Check if the parent GameObject itself matches the name
        if (parent.name == name)
        {
            return parent;
        }

        // Recursively search through all child GameObjects
        foreach (Transform child in parent.transform)
        {
            GameObject result = FindNestedGameObject(child.gameObject, name);
            if (result != null)
            {
                return result; // Return the found GameObject
            }
        }

        // Return null if no matching GameObject is found
        return null;
    }

    private IEnumerator DelayedPositionToHand()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds
        positionToHand();
    }
}
