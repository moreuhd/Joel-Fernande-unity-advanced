using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    private bool _isFogOn = false;
    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.fog = !RenderSettings.fog;
        Animator.SetTrigger("Close");
        _isFogOn = true;
        gameObject.SetActive(false);
    }

    
}
