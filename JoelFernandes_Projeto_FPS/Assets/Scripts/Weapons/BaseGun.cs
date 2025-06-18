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
    bool _canShoot;
    Queue<GameObject> holesQueue = new Queue<GameObject>(); 
    [SerializeField] private GameObject hole;

    public int Ammo { get => _ammo; set => _ammo = value; }



    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        Ammo = _maxAmmo;
        _timePassed = _fireRate;
    }
    private void Update()
    {
        if(_timePassed>=_fireRate)
        {
            _canShoot = true;
        }
        else
        {
            _timePassed += Time.deltaTime;
        }
    }
    #endregion


    public void Reload()
    {
        Ammo = _maxAmmo;
    }

    public void Fire()
    {
        print("Tryed to shoot");
        print(_timePassed + "when tryed to shoot");
        if(!_canShoot || Ammo <= 0)
        {
            return;
        }   

        _timePassed = 0;
        _canShoot = false;
        Ammo--;
         
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _gunRange))
        {

            IDamageable damageable = hit.transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1);
            }
            holesQueue.Enqueue(Instantiate(hole,hit.point,Quaternion.identity));
            if(holesQueue.Count>3)
            {
                Destroy(holesQueue.Dequeue());
            }
               

            //Add force to hit objects
            hit.rigidbody?.AddForce(-hit.normal * _impactForce, ForceMode.Impulse);

            //Create the impact object
            GameObject impact = Instantiate(_impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 1.0f);
        }
    }
}