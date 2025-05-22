#region Namespaces/Directives


using UnityEngine;
using UnityEngine.SceneManagement;

#endregion


public class GameManager : MonoBehaviour
{
	 private bool _hideCursor;
	 private static GameManager _instance;
	 public GameData gameData;
	 
	
	public Transform playerTransform;

	public void Save()
	{
	
    
		GameData saveFile = new GameData()
		{
			positionToSave[0] = playerTransform.position.x,
			positionToSave[1] = playerTransform.position.y,
			positionToSave[2] = playerTransform.position.z
		};
    
		try
		{
			SaveSystem.SaveGame(saveFile);
			
		}
		catch (System.Exception e)
		{
			
		}
	}

	public void Load()
	{
		try
		{
			GameData data = SaveSystem.LoadGame();
			if (data != null)
			{
				Vector3 position = new Vector3(data.positionToSave[0], data.positionToSave[1], data.positionToSave[2]);
			
				playerTransform.position = position;
			
			}
			else
			{
			
			}
		}
		catch (System.Exception e)
		{
		
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