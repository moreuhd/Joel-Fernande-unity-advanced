using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] protected int _ammo;
    [SerializeField] protected int _maxAmmo = 25;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public virtual void Reload()
    {



    }


    public virtual void Fire()
    {

    }
}
