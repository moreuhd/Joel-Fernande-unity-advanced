using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class events : MonoBehaviour
{
    [SerializeField] private Light _light;

    public void onDeath()
    {
        
    }

    public void onJump()
    {
        _light.color = new Color
        (
            UnityEngine.Random.Range(0.0f, 1.0f),
            UnityEngine.Random.Range(0.0f, 1.0f),
            UnityEngine.Random.Range(0.0f, 1.0f),
            1.0f
            );
    }



}
