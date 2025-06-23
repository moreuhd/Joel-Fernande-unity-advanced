#region Namespaces/Directives

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

    

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    protected override void Update()
    {
        base.Update();
        Move();
        Attack();   
    }

    #endregion

    private void Move()
    {
        if (_target != null)
        {
            Vector3 position = _target.transform.position;
            transform.LookAt(position);
            _agent.SetDestination(_target.transform.position);

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
            if (Vector3.Distance(_target.transform.position, transform.position) < 5)
            {
                _target.GetComponent<PlayerCharacter>().TakeDamage(1);

            }
        }
    }
}

