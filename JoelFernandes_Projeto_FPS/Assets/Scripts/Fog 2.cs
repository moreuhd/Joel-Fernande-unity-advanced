using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog2 : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.fog = !RenderSettings.fog;
        Animator.SetTrigger("Open");
    }
}
