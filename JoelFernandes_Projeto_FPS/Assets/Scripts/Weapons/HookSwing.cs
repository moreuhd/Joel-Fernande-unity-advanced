using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookSwing : MonoBehaviour
{
    [Header("References")]
    [SerializeField]private LineRenderer lineRenderer;
    [SerializeField] private Transform shootPoint, cam, player;
    [SerializeField] private LayerMask grappable;
    
    [Header("Swinging")]
    private float maxDistance = 100f;
    private Vector3 swingPoint;
    private SpringJoint joint;
    
   


    void update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse3)) StartSwing();
    }

    private void LateUpdate()
    {
      DrawRope(); 
    }

    private void StartSwing()
    {
        Debug.Log("aaaa");
        RaycastHit hit;
        if (Physics.Raycast(cam.position,cam.forward,out hit, maxDistance, grappable))
        {
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;
            
            float distanceFromPoint = Vector3.Distance(player.position, swingPoint);
            
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
            lineRenderer.positionCount = 2;
            currentGrapplePosition = shootPoint.position;
        }
    }
private Vector3 currentGrapplePosition;
    void DrawRope()
    {
        if(!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8);
        
        lineRenderer.SetPosition(0, shootPoint.position);
        lineRenderer.SetPosition(1, currentGrapplePosition);
    }
    
    private void StopSwing()
    {
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }
}
