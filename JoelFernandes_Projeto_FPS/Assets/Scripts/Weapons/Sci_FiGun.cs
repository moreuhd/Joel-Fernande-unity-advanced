using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sci_FiGun : MonoBehaviour
{
    #region Declarations

    [Header("Gun Settings")]
    [SerializeField] private float _gunRange;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _impactForce;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject firepoint;
    private float _timePassed;
    

    [SerializeField] int _ammo;
    [SerializeField] int _maxAmmo = 25;
    bool _canShoot;

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


    public void Reload()
    {
        Ammo = _maxAmmo;
    }

    public void Fire()
    {
        print("Tryed to shoot");
        print(_timePassed + "when tryed to shoot");
        if (!_canShoot || Ammo <= 0)
        {
            return;
        }

        _timePassed = 0;
        _canShoot = false;
        Ammo--;
        Instantiate(bullet, firepoint.transform.position, Camera.main.transform.rotation);
    }
}
