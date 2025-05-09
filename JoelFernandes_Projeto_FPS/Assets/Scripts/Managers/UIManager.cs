using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image _healthBar;

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


    public void HealthUpdate(int health)
    {
        _healthBar.fillAmount = health * 0.01f;
    }
}
