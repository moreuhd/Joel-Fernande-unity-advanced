using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour, IConsumeables
{
    [SerializeField] private int healthToRestore;
    public void Collect()
    {
        Debug.Log("restore 25 HP");

    }
}
