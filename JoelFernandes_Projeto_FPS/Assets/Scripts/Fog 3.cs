using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog3 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.fog = !RenderSettings.fog;
    }
}
