using UnityEngine;

public enum GameStates
{
	Preload,
	Setup,
	MainMenu,
	Playing,
	GameOver
};

public class GameManager : Singleton<GameManager>
{
#if UNITY_EDITOR
	public static readonly bool IN_EDITOR = true;
#else
	public static readonly bool IN_EDITOR = false;
#endif
#if UNITY_WEBPLAYER && !UNITY_EDITOR
	public static readonly bool IS_WEB = true;
#else
	public static readonly bool IS_WEB = false;
#endif

	#region public_fields
	public PlayerData playerData;
	public MazeData mazeData;
	public MazeCellData mazeCellData;
	public ElevatorData elevatorData;
	public GUISkin skin;
	#endregion

	#region private_fields
	private GameStates _state = GameStates.Preload;
	private PlayerBehaviour _player;
	private MazeManager _maze;
	private bool _playingInit = false;

	#endregion

	#region monobehaviour
	protected new void Awake()
	{
		base.Awake();
		DontDestroyOnLoad(this);
	}
	
	private void Start()
	{
		_maze = new MazeManager();
	}
	
	private void Update()
	{
		switch(_state)
		{
		case GameStates.Preload:
			preloadUpdate();
			break;
		case GameStates.Setup:
			if(Time.timeSinceLevelLoad >= 2f)
			{
				finishSetup();
			}
			break;
		case GameStates.MainMenu:
			break;
		case GameStates.Playing:
			playingUpdate();
			break;
		case GameStates.GameOver:
			GameObject.FindGameObjectWithTag("WinScreenGUI").GetComponent<GUITexture>().enabled = true;
			break;
		default:
			break;
		}
	}
	
	private void OnLevelWasLoaded()
	{
		if(_state == GameStates.Setup && !_playingInit)
		{
			setupMaze();
			_playingInit = true;
		}
	}

	private void OnGUI()
	{
		if(skin != null && GUI.skin != skin)
		{
			GUI.skin = skin;
		}
		switch(_state)
		{
		case GameStates.Playing:
			break;
		case GameStates.GameOver:
			break;
		default:
			GUI.backgroundColor = Color.black;
			GUILayout.BeginArea(new Rect(Screen.width/6f, Screen.height/6f, 2*Screen.width/3, 2*Screen.height/3));
			GUILayout.Box("Peter Bartosch\npresents\n\nCubey's Confusion Cube", skin.GetStyle("title"), GUILayout.ExpandWidth(true));
			GUILayout.EndArea();
			break;
		}
	}
	#endregion
	#region setups
	private void setupMaze()
	{
		_maze.Init(mazeData, mazeCellData);
		ElevatorColor = _maze.SpawnPickups();
	}
	
	private void setupPlayer()
	{
		GameObject tmpGO = GameObject.Instantiate(playerData.playerPrefab,
		                                          _maze.EndPosition,
		                                          Quaternion.identity)
													as GameObject;
		gameObject.GetComponent<AudioListener>().enabled = false;
		_player = tmpGO.GetComponent<PlayerBehaviour>();
		_player.Init(playerData, _maze.EndPosition);
	}
	
	private void finishSetup()
	{
		setupPlayer();
		_maze.CanDoPickup = true;
		_state = GameStates.Playing;
	}
	#endregion

	/* preloadUpdate()
	 * This is the update method for the Preload GameState.  We only care about making sure levels can be loaded
	 * if we're using the web player, so check to see if we can do this yet.  As long as we cannot load the level
	 * do nothing.  Once we can, load it.
	 * 
	 */
	private void preloadUpdate()
	{
		if(!Application.isLoadingLevel && ((IS_WEB && Application.CanStreamedLevelBeLoaded(1)) || !IS_WEB))
		{
			Application.LoadLevel(1);
			_state = GameStates.Setup;
		}
		else if(!Application.isLoadingLevel && IS_WEB)
		{
			int pct = Mathf.FloorToInt(100*Application.GetStreamProgressForLevel(1));
			//update something on a gui?
		}
	}
	
	private void playingUpdate()
	{
		checkAndHandleInputs();
		if(_player.IsOnElevator && !ElevatorBehaviour.IsAtEnd)
		{
			_player.MoveUp(elevatorData.defaultSpeed);
		}
	}
	
	#region input_handling
	private void checkAndHandleInputs()
	{
		if(Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
		else
		{
			float vert = Input.GetAxis("Vertical");
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");
			
			switch(_state)
			{
			case GameStates.Preload:
				break;
			case GameStates.Setup:
				break;
			case GameStates.MainMenu:
				break;
			case GameStates.Playing:
				playingWASD((vert>0), (vert<0));
				playingMouse(mouseX, mouseY);
				if(IN_EDITOR)
				{
					if(Input.GetKeyDown(KeyCode.Alpha1))
					{
						_player.AddScore(Color.red);
					}
					else if(Input.GetKeyDown(KeyCode.Alpha2))
					{
						_player.AddScore(new Color(1f, 1f, 0f));
					}
					else if(Input.GetKeyDown(KeyCode.Alpha3))
					{
						_player.AddScore(Color.blue);
					}
				}
				break;
			case GameStates.GameOver:
				break;
			default:
				break;
			}
		}
	}
	
	private void playingWASD(bool fwd, bool back)
	{
		_player.MoveOnGround(fwd, back);
	}
	
	private void playingMouse(float x, float y)
	{
		//Rotation around y-axis is dependent on horizontal movement
		_player.RotateY(x);

		//Rotation around x-axis is dependent on vetical movement
		_player.RotateX(y);
	}
	#endregion
	
	#region collision_handle
	public void PlayerPickupCollision(ControllerColliderHit c)
	{
		if(_maze.CanDoPickup && c.gameObject.CompareTag("Pickup"))
		{
			_maze.CanDoPickup = false;
			AudioSource.PlayClipAtPoint(c.gameObject.audio.clip, c.gameObject.transform.position);
			if(!_player.AddScore(_maze.RemovePickups(c.gameObject)))
			{
				ElevatorColor = _maze.SpawnPickups();
				_maze.CanDoPickup = true;
			}
			else
			{
				ElevatorColor = new Color(0f, 0.7f, 0f);
				if(c.gameObject != null)
				{
					c.gameObject.SetActive(false);
				}
			}
		}
		else if(c.gameObject.CompareTag("EndPickup"))
		{
			_state = GameStates.GameOver;
		}
	}
	#endregion	

	public void ShowEnd()
	{
		_maze.ShowEnd();
	}
	
	public GameStates State	{ get { return _state; } }

	public Color ElevatorColor { get; private set; }

	public int[] Score
	{
		get
		{
			if(_player != null)
			{
				return _player.Score;
			}
			else
			{
				return new int[] {0, 0, 0};
			}
		}
	}
}