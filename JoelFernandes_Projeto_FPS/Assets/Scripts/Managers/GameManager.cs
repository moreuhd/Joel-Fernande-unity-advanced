#region Namespaces/Directives


using UnityEngine;
using UnityEngine.SceneManagement;

#endregion


public class GameManager : MonoBehaviour
{
	 private bool _hideCursor;
	 private static GameManager _instance;
	
	public Transform playerTransform;

	public void Save()
	{
		GameData data = new GameData(playerTransform.position);
		SaveSystem.SaveGame(data);
	}

	public void Load()
	{
		GameData data = SaveSystem.LoadGame();
		if (data != null)
		{
			
			
			Vector3 position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
			playerTransform.position = position;
		}
	}
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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F5)) 
		{
			Save();
		}

		if (Input.GetKeyDown(KeyCode.F6))
		{
			Load();
		}
	}
}

