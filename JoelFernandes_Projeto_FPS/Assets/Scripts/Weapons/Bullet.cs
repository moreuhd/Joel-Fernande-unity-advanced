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

    private void OnCollisionEnter(Collision collision)
    {
        print("Collision with: " + collision.collider.name);
        if (collision.collider.TryGetComponent(out IDamageable damageable))
        {
            damageable.Knockback((transform.position - collision.contacts[0].normal).normalized, 5);
            damageable.TakeDamage(10);
        }
        Destroy(gameObject);
    }
    
}
