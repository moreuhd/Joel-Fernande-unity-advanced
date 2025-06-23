using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sci_FiGun : Gun
{
    #region Declarations

    [Header("Gun Settings")]
    [SerializeField] private float _gunRange;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _impactForce;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject firepoint;
    private float _timePassed;
    
    bool _canShoot;




    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        _ammo = _maxAmmo;
        _timePassed = _fireRate;
    }
    private void Update()
    {
        if (_timePassed >= _fireRate)
        {
            _canShoot = true;
        }
        else
        {
            _timePassed += Time.deltaTime;
        }
        
    }
    #endregion

     public override void Reload()
    {
        _ammo = _maxAmmo;
    }

    public void Fire()
    {
        print("Tryed to shoot");
        print(_timePassed + "when tryed to shoot");
        if (!_canShoot || _ammo <= 0)
        {
            return;
        }

        _timePassed = 0;
        _canShoot = false;
        _ammo--;
        Instantiate(bullet, firepoint.transform.position, Camera.main.transform.rotation);
    }
}
