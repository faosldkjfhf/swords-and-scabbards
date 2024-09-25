using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Blade
{
    // Damage of the weapon
    Damage damage { get; set; }

    // x = top weight, y = bottom weight
    Vector2 weight { get; set; }
}
