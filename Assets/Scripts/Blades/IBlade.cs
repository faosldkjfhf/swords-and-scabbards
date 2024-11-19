using UnityEngine;

public interface IBlade
{
    float DamageValue { get; set; }

    float WeightValue { get; set; }

    GameObject HandleConnectionPoint { get; set; }
}
