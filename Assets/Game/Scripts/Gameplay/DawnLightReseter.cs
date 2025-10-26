using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DawnLightReseter : MonoBehaviour
{
    private Light2D dawnLight;

    private void Start()
    {
        dawnLight = GetComponent<Light2D>();
    }

    public void ResetLight()
    {
        dawnLight.intensity = 0;
    }

}
