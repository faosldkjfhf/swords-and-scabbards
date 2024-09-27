using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleHandle : MonoBehaviour, IHandle
{
    [Header("Handle Properties")]
    [SerializeField]
    private MonoBehaviour swingStyle;
    [SerializeField]
    private float dropChancePercentage;

    public ISwingStyle SwingStyle
    {
        get { return (ISwingStyle)swingStyle; }
        set { swingStyle = (MonoBehaviour)value; }
    }

    public float DropChancePercentage
    {
        get { return dropChancePercentage; }
        set { dropChancePercentage = value; }
    }
}
