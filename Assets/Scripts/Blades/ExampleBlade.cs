using System.Collections;
using System.Collections.Generic;
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
    private GameObject handleConnectionPoint;

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

}