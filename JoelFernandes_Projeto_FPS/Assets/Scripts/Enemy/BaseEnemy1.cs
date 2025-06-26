#region Namespaces/Directives

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#endregion

public class BaseEnemy1 : BrainAdvanced, IDamageable
{
    #region Declarations

    [Header("Movement Settings")]
    [SerializeField] private NavMeshAgent _agent;    
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _health = 3;
    [SerializeField] private Rigidbody _rigidbody;
    private bool _canMove = true;
    

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Move();
    }

    void Update()
    {
        Attack();
    }

    #endregion

    private void Move()
    {
        if (_canMove == false)
        {
            return;
        }
        if (_target != null)
            {
                Vector3 direction = (_target.position - transform.position).normalized;
                _rigidbody.velocity = direction * _movementSpeed;


            }
            else
            {
                _rigidbody.velocity = Vector3.zero;
            }

    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Attack()
    {
        if (_target != null)
        {
            if (Vector3.Distance(_target.transform.position, transform.position) < 2)
            {
                _target.GetComponent<PlayerCharacter>().TakeDamage(1);  

            }
        }
    }



    public void Knockback(Vector3 direction, float force)
    {
        print("Knockback called");
        _target = null;
        _canMove = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        StartCoroutine(KnockbackCooldown());
    }

    private IEnumerator KnockbackCooldown()
    {
        yield return new WaitForSeconds(1f);
        _canMove = true;
        _rigidbody.velocity = Vector3.zero;
    }
}

