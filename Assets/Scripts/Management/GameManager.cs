using UnityEngine;
using System.Collections;

public enum GameStates
{
	MainMenu = 0x01,
	Playing = 0x02,
	GameOver = 0x04
};

public class GameManager
{
	private static GameManager _instance = null;
	
	public static GameManager Instance
	{
		get
		{
			if(GameManager._instance == null)
				GameManager._instance = new GameManager();
			return GameManager._instance;
		}
	}
	
	public static GameStates State { get { return GameManager.Instance._state; } }
	
	private GameStates _state;
	
	//Other managers
	private PlayerManager _player;
	private MazeManager _maze;
	private ElevatorManager _elevator;
	
	private GameObject _inputGO;
	
	//NOTE: this is private
	private GameManager()
	{
		_player = null;
		_maze = null;
		_elevator = null;
		Screen.showCursor = false;
	}
	
	public void Init(PlayerData pData, MazeData mData, MazeCellData mcData, ElevatorData eleData)
	{
		_inputGO = GameObject.Find("_Input");
		_inputGO.SetActive(false);
		_state = GameStates.Playing; //Testing only
		
		_maze = new MazeManager();
		_maze.Init(mData, mcData);
		
		_player = new PlayerManager();
		_player.Init(pData, _maze.EndPosition);
		
		_elevator = new ElevatorManager(eleData);
		_elevator.ElevatorColor = _maze.SpawnPickups();
	}
	
	public bool FinishInit()
	{
		if(_player == null)
			return false;
		
		GameObject.DestroyImmediate(GameObject.Find("SplashScreen"));
		_player.Activate();
		
		//turn input on after all init to avoid null stuffs
		_inputGO.SetActive(true);
		_maze.CanDoPickup = true;
		return true;
	}
	
	#region wasd_handle
	public void HandleKeyboard(float horiz, float vert)
	{
		switch(_state)
		{
		case GameStates.MainMenu:
			break;
		case GameStates.Playing:
			PlayingWASD((vert>0), (vert<0), (horiz>0), (horiz<0));
			break;
		case GameStates.GameOver:
			break;
		default:
			break;
		}
	}
	
	private void PlayingWASD(bool fwd, bool back, bool right, bool left)
	{
		_player.Move(fwd, back, right, left);
	}
	#endregion
	
	#region mouse_handle
	public void HandleMouse(float x, float y)
	{
		switch(_state)
		{
		case GameStates.MainMenu:
			break;
		case GameStates.Playing:
			PlayingMouse(x,y);
			break;
		case GameStates.GameOver:
			break;
		default:
			break;
		}
	}
	
	private void PlayingMouse(float x, float y)
	{
		_player.RotateY(x);
		_player.RotateX(y);
	}
	#endregion
	
	#region other_keyboard
	public void HandleEscape(bool esc)
	{
		if(esc)
			Application.Quit();
	}
	#endregion
	
	#region collision_handle
	public void PlayerCollideHandle(ControllerColliderHit c)
	{
		//since this should only ever happen when playing, no need to switch
		
		//filter type of hit before passing to PlayerManager
		//plus we need to do some things here
		if(_maze.CanDoPickup && c.gameObject.tag.Equals("Pickup",System.StringComparison.OrdinalIgnoreCase))
		{
			_maze.CanDoPickup = false;
			AudioSource.PlayClipAtPoint(c.gameObject.audio.clip, c.gameObject.transform.position);
			_player.AddScore(_maze.RemovePickups(c.gameObject));
			_elevator.ElevatorColor = _maze.SpawnPickups();
			_maze.CanDoPickup = true;
		}
		else if(c.gameObject.tag.Equals("EndPickup",System.StringComparison.OrdinalIgnoreCase))
		{
			GameObject.FindGameObjectWithTag("WinScreenGUI").GetComponent<GUITexture>().enabled = true;
		}
	}
	
	public void ElevatorTrigger(Vector3 position, bool enter)
	{
		if(enter)
			_elevator.SpawnNewElevator(position);
		else
			_elevator.RemoveChild();
	}
	#endregion
	
	public void MoveElevator()
	{
		if(_elevator.IsMovingUp)
			_elevator.MoveUp();
	}
	
	public void ShowEnd()
	{
		_maze.ShowEnd();
	}
}