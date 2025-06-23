using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image _healthBar;

    [SerializeField] private Text ammo1;
    [SerializeField] private Text ammo2;
    [SerializeField] private BaseGun gun1;
    [SerializeField] private Sci_FiGun gun2;

    private static UIManager _instance;

    public static UIManager Instance { get => _instance; set => _instance = value; }

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    public void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void Update()
    {
        //ammo1.text = gun1.Ammo.ToString();
        //ammo2.text = gun2.Ammo.ToString();
    }


    public void HealthUpdate(int health)
    {
        _healthBar.fillAmount = health * 0.01f;
    }
}
