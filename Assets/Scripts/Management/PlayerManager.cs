using UnityEngine;
using System.Collections;

public class PlayerManager
{
	PlayerData _data;
	GameObject _playerGameObject;
	ScoreCubesBehaviour _scoreDisplay;
	int _redScore;
	int _yellowScore;
	int _blueScore;
	bool _endShown;
	
	public PlayerManager()
	{
		_data = null;
		_playerGameObject = null;
		_redScore = 0;
		_yellowScore = 0;
		_blueScore = 0;
		_endShown = false;
	}
	
	public void Init(PlayerData data, Vector3 position)
	{
		if(data == null)
			_data = new PlayerData();
		else
			_data = data;
		
		_playerGameObject = (GameObject) GameObject.Instantiate(data.playerPrefab, position, Quaternion.identity);
		
		CharacterController ctrl = _playerGameObject.GetComponent<CharacterController>();
		if(ctrl == null)
			ctrl = _playerGameObject.AddComponent<CharacterController>();
		ctrl.height = 2.0f;
		ctrl.radius = 0.75f;
		
		_playerGameObject.transform.position = position;
		_playerGameObject.SetActive(false);
	}
	
	public void Activate()
	{
		_playerGameObject.SetActive(true);
		GameObject tmp = (GameObject)GameObject.Instantiate(_data.scoreDisplayPrefab, (new Vector3(1000,0,1000)), Quaternion.identity);
		_scoreDisplay = tmp.GetComponent<ScoreCubesBehaviour>();
	}
	
	public void Move(bool moveFwd, bool moveBack, bool moveRight, bool moveLeft)
	{
		Vector3 mvmnt = new Vector3();
		CharacterController ctrl = _playerGameObject.GetComponent<CharacterController>();
		if(moveFwd || moveBack)
			MoveZ(moveBack, ref mvmnt);
		if(moveRight || moveLeft)
			MoveX(moveLeft, ref mvmnt);
		
		mvmnt*=_data.speed;
		
		ApplyGravity(ref mvmnt);
		
		ctrl.Move (mvmnt*Time.deltaTime);	
	}
	
	private void MoveZ(bool moveBack, ref Vector3 mvmnt)
	{
		Vector3 relFwd = _playerGameObject.transform.TransformDirection(Vector3.forward);
		mvmnt+=relFwd * ((moveBack)?-1:1);// * _data.tilesPerMove;
	}
	
	private void MoveX(bool moveLeft, ref Vector3 mvmnt)
	{
		Vector3 relRight = _playerGameObject.transform.TransformDirection(Vector3.right);
		mvmnt+=relRight * ((moveLeft)?-1:1);// * _data.tilesPerMove;
	}
	
	private void ApplyGravity(ref Vector3 mvmnt)
	{
		mvmnt.y-=_data.gravity*Time.deltaTime;
	}
	
	public void RotateY(float movement)
	{
		_playerGameObject.transform.Rotate(Vector3.up * _data.turnSpeed*movement, Space.Self);
	}
	
	public void RotateX(float movement)
	{
		GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
		
		cam.transform.Rotate(-Vector3.right * _data.turnSpeed*movement, Space.Self);
	}
	
	public void AddScore(Color c)
	{
		Vector3 red = Vector3.right;
		Vector3 blue = Vector3.forward;
		Vector3 yellow = Vector3.right + Vector3.up;
		Vector3 cv = new Vector3(c.r, c.g, c.b);
		
		if(cv == red)
		{
			if(_redScore++ < 6)
				_scoreDisplay.AddOne(ScoreCubesBehaviour.ScoreCubeColor.Red);
		}
		else if(cv == yellow)
		{
			if(_yellowScore++ < 6)
				_scoreDisplay.AddOne(ScoreCubesBehaviour.ScoreCubeColor.Yellow);
		}
		else if(cv == blue)
		{
			if(_blueScore++ < 6)
				_scoreDisplay.AddOne(ScoreCubesBehaviour.ScoreCubeColor.Blue);
		}
		
		if(!_endShown && _redScore>=6 && _yellowScore>=6 && _blueScore>=6)
		{
			_scoreDisplay.TurnOnExitSign();
			GameManager.Instance.ShowEnd();
			_endShown = true;
		}
	}
}