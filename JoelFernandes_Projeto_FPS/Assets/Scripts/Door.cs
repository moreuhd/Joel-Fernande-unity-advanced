using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.fog = !RenderSettings.fog;
        animator.SetTrigger("Open");
    }


}
