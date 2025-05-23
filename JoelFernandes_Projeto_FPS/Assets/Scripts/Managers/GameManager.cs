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

	public void Save(GameData saveFile)
	{
		saveFile.positionToSave[0] = playerTransform.position.x;
		saveFile.positionToSave[1] = playerTransform.position.y;
		saveFile.positionToSave[2] = playerTransform.position.z;
	}

	public void Load(GameData saveFile)
	{
			if (saveFile != null)
			{
				Vector3 position = new Vector3(saveFile.positionToSave[0], saveFile.positionToSave[1], saveFile.positionToSave[2]);
			
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
        SaveSystem.onSave.AddListener(Save);
        SaveSystem.onLoad.AddListener(Load);
        
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
			SaveSystem.SaveGame();
		}

		if (Input.GetKeyDown(KeyCode.F6))
		{
			SaveSystem.LoadGame();
		}
	}
}