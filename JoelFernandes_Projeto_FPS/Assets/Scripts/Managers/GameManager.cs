#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion


public class GameManager : MonoBehaviour
{
	[SerializeField] private bool _hideCursor;
	[SerializeField] private static GameManager _instance;

    public static GameManager Instance { get => _instance; set => _instance = value; }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    void Start()
	{
		if (_hideCursor)
        {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}			
	}


	public void PlayerDeath()
	{

		SceneManager.LoadScene(2);
	}



}

