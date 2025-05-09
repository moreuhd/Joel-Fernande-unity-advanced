#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Pool;

#endregion

public class BaseEnemy : MonoBehaviour, IDamageable
{
    #region Declarations

    [Header("Movement Settings")]
    [SerializeField] private Vector3 _patrolPositionOne;
    [SerializeField] private Vector3 _patrolPositionTwo;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int Health = 3;
    [SerializeField] private GameObject _shot;
    [SerializeField] private float _shotSpeed;
    private Rigidbody Rigidbody;
    private Collider Collider;
    private Transform _target;
    private NormalShot normalShot;
    private Vector3 _targetPosition;
    [SerializeField] private LayerMask _layerMask;
    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        normalShot = GetComponent<NormalShot>();
        Collider = GetComponent<Collider>();
        Rigidbody = GetComponent<Rigidbody>();
        _targetPosition = _patrolPositionOne;
    }

    private void Update()
    {
        if (Rigidbody == null) return;
        Move();
        PlayerDetection();
        death();
    }

    #endregion

    private void Move()
    {

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _movementSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _targetPosition) < 0.2f)
        {
            if (_targetPosition == _patrolPositionOne)
            {
                _targetPosition = _patrolPositionTwo;
            }
            else
            {
                _targetPosition = _patrolPositionOne;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Grab(Transform grabber)
    {
        Collider.enabled = false;
        Destroy(Rigidbody);
        //_barrelRigidbody.velocity = Vector3.zero;   
        //_barrelRigidbody.useGravity = false;
        transform.localPosition = Vector3.zero;
    }


    private void PlayerDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 20);
        foreach (Collider detection in colliders)
        {
            if (detection.gameObject.GetComponent<PlayerCharacter>())
            {
                _target = detection.transform;
            }

        }
        if (_target == null)
        {
            return;
        }


        if (Vector3.Distance(transform.position, _targetPosition) < 5)
        {
            _target = null;
        }
        if (_target != null)
        {
            _timePassed += Time.deltaTime;

            if (_timePassed >= _timeToPass)
            {
                _timePassed = 0;

                Vector3 direction = _target.position - transform.position;
                direction.Normalize();
                StartCoroutine(bullets());
            }

        }



    }

    private float _timePassed = 0;
    private float _timeToPass = 1;
    private void OnCollisionEnter(Collision collision)
    {
        Health--;

    }

    private void death()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }


    private IEnumerator bullets()
    {
        yield return new WaitForSeconds(1);
        GameObject bullet = bulletpool.instance.GetPooledObject();
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        bullet.SetActive(false);

    }

}

