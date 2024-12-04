using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class ExampleBlade : MonoBehaviour, IBlade
{
    [Header("Blade Properties")]
    [SerializeField]
    private float damageValue;

    [SerializeField]
    private float weightValue;

    [Header("Blade Connection Point")]
    [SerializeField]
    public GameObject handleConnectionPoint;

    public float DamageValue
    {
        get { return damageValue; }
        set { damageValue = value; }
    }

    public float WeightValue
    {
        get { return weightValue; }
        set { weightValue = value; }
    }

    public GameObject HandleConnectionPoint
    {
        get { return handleConnectionPoint; }
        set { handleConnectionPoint = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        EmptyWeapon weapon = this.GetComponentInParent<EmptyWeapon>();
        int selfId = this.GetComponentInParent<PlayerController>().GetInstanceID();
        int otherId = other.GetComponent<PlayerController>().GetInstanceID();
        if (weapon.GetAttacking() && !weapon.DealtDamage())
        {
            if (other.gameObject.CompareTag("Player") && otherId != selfId)
            {
                // if other player is blocking, they don't take damage
                if (
                    other
                        .gameObject.GetComponentInChildren<TwoDimensionalAnimationStateController>()
                        .GetIsBlocking()
                )
                {
                    Debug.Log("BLOCKED!");
                    return;
                }

                // take damage

                other
                    .gameObject.GetComponent<PlayerController>()
                    .TakeDamage(damageValue, weapon.GetAttackType());
                weapon.SetDealtDamage(true);
            }
        }
    }
}
