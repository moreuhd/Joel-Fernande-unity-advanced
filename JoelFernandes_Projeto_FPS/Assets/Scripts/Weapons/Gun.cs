using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [SerializeField] protected int _ammo;
    [SerializeField] protected int _maxAmmo = 25;
    [SerializeField] protected Text _ammoText;
    [SerializeField] protected float _gunRange;
    [SerializeField] protected float _fireRate;
    [SerializeField] protected float _impactForce;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public virtual void Reload()
    {
        _ammo = _maxAmmo;
        UpdateUI();
    }
    void UpdateUI()
    {
        _ammoText.text = _ammo.ToString();
    }

    public virtual void Fire()
    {
        UpdateUI();
    }
}
