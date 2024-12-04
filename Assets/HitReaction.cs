using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitReaction : MonoBehaviour
{
    public Transform headBone; // Drag the head bone here in the inspector.
    public float snapBackSpeed = 5f; // Adjust for smoothness.
    public Vector3 hitRotation = new Vector3(-30, 0, 0); // Example rotation when hit.

    private Quaternion originalRotation;
    private bool isHit = false;

    void Start()
    {
        if (headBone != null)
        {
            originalRotation = headBone.localRotation;
        }
    }

    void Update()
    {
        if (isHit)
        {
            // Gradually return to the original rotation
            headBone.localRotation = Quaternion.Lerp(headBone.localRotation, originalRotation, Time.deltaTime * snapBackSpeed);

            // Stop the motion once it's back to the original position
            if (Quaternion.Angle(headBone.localRotation, originalRotation) < 0.1f)
            {
                isHit = false;
            }
        }
    }

    public void OnHit()
    {
        if (headBone != null)
        {
            // Apply the hit rotation
            headBone.localRotation = Quaternion.Euler(hitRotation);
            isHit = true;
        }
    }
}