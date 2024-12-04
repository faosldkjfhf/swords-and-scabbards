using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKConstraintController : MonoBehaviour
{
    public TwoBoneIKConstraint rightHandGrabWeapon;
    public TwoBoneIKConstraint leftHandGrabWeapon;

    // These are the targets and hints that will be assigned
    public Transform rightHandTarget;
    public Transform leftHandTarget;

    void Start()
    {
        AssignIKTargets();
    }

     void Update()
    {

    }

    void AssignIKTargets()
    {
        // if (rightHandGrabWeapon != null && rightHandTarget != null)
        // {
        //     Debug.LogError("did the thing:" + rightHandTarget.name);
        //     // Assign the target and hint for the right hand IK
        //     rightHandGrabWeapon.data.target.transform.position = rightHandTarget.transform.position;
        //     rightHandGrabWeapon.data.target.rotation = rightHandTarget.rotation;


        // }

        // if (leftHandGrabWeapon != null && leftHandTarget != null)
        // {
        //     // Assign the target and for the left hand IK
        //     leftHandGrabWeapon.data.target.transform.position = leftHandTarget.transform.position;
        //     leftHandGrabWeapon.data.target.rotation = leftHandTarget.rotation;
        //     Debug.LogError("did the thing but on the left:" + rightHandTarget.name);
        // }

        // if(leftHandTarget == null)
        // {
        //     leftHandGrabWeapon.enabled = false;
        // }
    }
}