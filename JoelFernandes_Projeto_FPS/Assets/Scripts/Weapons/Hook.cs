using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerCharacter _character;
    [SerializeField] private Transform  cam;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private LayerMask _grapable;
    
    
    [Header("Grappling")]
    [SerializeField] private float  _maxDistance;
    [SerializeField] private float _delay;
    private Vector3 _grapplePoint;
    [SerializeField] private float overShootYAxis;
    
    [Header("Cooldown")]
    [SerializeField] private float _cooldown;
    [SerializeField] private float _cooldownTimer;
    private bool _grappling;
    [SerializeField] private LineRenderer _lineRenderer;
    

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Mouse1)) StartGrapple();
        
        if (_cooldownTimer > 0)
        _cooldownTimer -= Time.deltaTime;
            
        
    }

    private void LateUpdate()
    {
        if(_grappling)
            _lineRenderer.SetPosition(0, _firePoint.position);
    }

    private void StartGrapple()
    {
        if (_cooldownTimer > 0) return;
        
        _grappling = true; 
        
        _character.Freeze = true;
        
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, _maxDistance, _grapable))
        {
            _grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), _delay);
           
        }
        else
        {
            _grapplePoint = cam.position + cam.forward * _maxDistance;
            Invoke(nameof(StopGrapple), _delay);
        }
        
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(1, _grapplePoint);
    }

    private void ExecuteGrapple()
    {
        _character.Freeze = false;
        
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 2f , transform.position.z);
        
        float grapplePointRelativeYPos = _grapplePoint.y -lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overShootYAxis;
            
        _character.JumpToPosition(_grapplePoint, highestPointOnArc);
            
        Invoke(nameof(StopGrapple), 4f);
    }

    public void StopGrapple()
    {

        _grappling = false;
        _character.Freeze = false;
        _cooldownTimer = _cooldown;
        
        _lineRenderer.enabled = false;
    }
    
   
}
