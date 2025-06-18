using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _bSpeed;
    [SerializeField] private LayerMask _layerMask;

    private void Awake()
    {
       Destroy(gameObject, 5); 
    }

    void Start()
    {
        _rb.velocity = transform.forward * _bSpeed;
    }

    private void OncollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<IDamageable>() != null && collision.collider.gameObject.layer == _layerMask)
        {
            collision.collider.GetComponent<IDamageable>().TakeDamage(10);
            
        }
        Destroy(gameObject);
    }
    
}
