using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandle
{
    // Property for an ISwingStyle object
    ISwingStyle SwingStyle { get; set; }

    // Property for the drop chance percentage
    float DropChancePercentage { get; set; }
}
