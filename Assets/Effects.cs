using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public Material screenDamageMat;
    private static Coroutine screenDamageTask;
    float speed = 5.0f;

    public void ScreenDamageEffect(float intensity)
    {
        if (screenDamageTask != null)
        {
            StopCoroutine(screenDamageTask);
        }
        screenDamageTask = StartCoroutine(screenDamage(intensity));
    }

    IEnumerator screenDamage(float intensity)
    {
        Debug.Log("screen damage effect");
        var targetRadius = Remap(intensity, 0, 1, 0.4f, -0.15f);
        float currRadius = 1.0f;
        for (float t = 0; currRadius != targetRadius; t += speed * Time.deltaTime)
        {
            currRadius = Mathf.Lerp(1, targetRadius, t);
            screenDamageMat.SetFloat("_VignetteRadius", currRadius);
            yield return null;
        }

        for (float t = 0; currRadius < 1; t += Time.deltaTime)
        {
            currRadius = Mathf.Lerp(targetRadius, 1, t);
            screenDamageMat.SetFloat("_VignetteRadius", currRadius);
            yield return null;
        }
    }

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
    }
}
