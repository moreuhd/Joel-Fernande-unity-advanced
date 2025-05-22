using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    private Rigidbody _rigidBody;
    [SerializeField] private LayerMask _layerMask;
    PlayerCharacter _character;
    Vector3 _targetPosition;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();

    }

    private void Start()
    {
        _character = PlayerCharacter.Instance;
    }

    private void Update()
    {
        
    }
    public void Movement(Vector3 direction)
    {
        _rigidBody.velocity = direction.normalized * _movementSpeed;
    }



    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("hooked"))
        {
            _targetPosition = collision.gameObject.transform.position;
            Vector3 direction = _character.transform.position - transform.position;
            direction.Normalize();

            

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.transform.GetComponent<PlayerCharacter>() != null)
                {
                    _character.LineRenderer.enabled = true;
                    _character.Attract(_targetPosition);
                }
            }


        }
        else if(collision.gameObject.layer != LayerMask.NameToLayer("hooked"))
        {
            _character.LineRenderer.enabled = false;
        }
        
        Destroy(gameObject);




    }
}
