#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class BaseGun : MonoBehaviour
{
    #region Declarations

    [Header("Gun Settings")]
    [SerializeField] private float _gunRange;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _impactForce;
    [SerializeField] private GameObject _impactEffect;
    private float _timePassed;

    [SerializeField] int _ammo;
    [SerializeField] int _maxAmmo = 25;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        _ammo = _maxAmmo;
        _timePassed = _fireRate;
    }

    #endregion


    public void Reload()
    {
        _ammo = _maxAmmo;
    }

    public void Fire()
    {
        if(_timePassed < _fireRate)
        {
            _timePassed += Time.deltaTime;
        }
        else
        {
            if(_ammo == 0)
            {
                return;
            }

            _timePassed = 0;
            _ammo--;
         
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _gunRange))
            {

                IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(1);
                }


                Debug.Log("We hit something");

                //Add force to hit objects
                hit.rigidbody?.AddForce(-hit.normal * _impactForce, ForceMode.Impulse);

                //Create the impact object
                GameObject impact = Instantiate(_impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 1.0f);
            }
        }
    }
}
