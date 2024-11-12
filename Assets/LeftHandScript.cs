using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LeftHandScript : MonoBehaviour
{


    public TwoBoneIKConstraint ikConstraint; // Reference to the Two Bone IK Constraint
    private Transform leftHandGrip;
    GameObject parent;
    bool triedLeftHand = false;

    // Start is called before the first frame update
    void Start()
    {
        // Find the parent GameObject
        ikConstraint = GetComponent<TwoBoneIKConstraint>();
        parent = transform.parent?.gameObject;
        if (parent == null)
        {
            Debug.LogError("Parent GameObject not found!");
            return;
        }

        // Search for the "leftHandGrip" GameObject in the parent's hierarchy
        
    }


    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            Transform found = FindDeepChild(child, childName);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    private void setLeftHand()
    {
        leftHandGrip = FindDeepChild(parent.transform, "leftHandGrip");

        if (leftHandGrip == null && !triedLeftHand)
        {
            triedLeftHand = true;
            Debug.LogError("leftHandGrip GameObject not found in the " + parent.name + " hierarchy!");
            return;
        }

        // Assign the leftHandGrip as the target of the IK Constraint
        if (ikConstraint != null && !triedLeftHand)
        {
            //ikConstraint.data.target = leftHandGrip;
            Debug.LogError("IK Constraint target successfully set to leftHandGrip.");
            triedLeftHand = true;
        }
        else if (ikConstraint == null)
        {
            Debug.LogError("Two Bone IK Constraint reference not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        setLeftHand();
        transform.position = leftHandGrip.transform.position;
    }
}
