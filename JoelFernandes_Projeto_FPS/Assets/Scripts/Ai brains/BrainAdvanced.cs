using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainAdvanced : MonoBehaviour
{
	protected Transform _target;
	private List<Transform> targetsInMemory;

	[SerializeField] protected float _maxDistance;
	[SerializeField] protected float _coneAngle;
	


	protected virtual void Update()
	{
		Search();
	}

	void Search()
	{
		Collider[] hitColliders = new Collider[100];
		targetsInMemory = new List<Transform>();
		hitColliders = Physics.OverlapSphere(transform.position, _maxDistance);

		for (var i = 0; i < hitColliders.Length; i++)
		{
			Transform tempTarget = hitColliders[i].transform;

			Vector3 dir = tempTarget.position - transform.position; // find target direction
			if (Vector3.Angle(dir, transform.forward) <= _coneAngle / 2)
			{
			
				PlayerCharacter player = tempTarget.GetComponent<PlayerCharacter>();
				if (player != null)
					_target = player.transform;
			}
		}


	}	
}
