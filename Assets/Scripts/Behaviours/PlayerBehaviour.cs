using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerBehaviour : MonoBehaviour
{	
	private CharacterController _charCtrl;
	private PlayerData _data;
	private ScoreCubesBehaviour _scoreDisplay;
	private int _redScore = 0;
	private int _yellowScore = 0;
	private int _blueScore = 0;
	private bool _endShown = false;
	private bool _onElevator = false;
	
	#region monobehaviour	
	private void Start()
	{
		_charCtrl = gameObject.GetComponent<CharacterController>();
	}
	
	private void OnControllerColliderHit(ControllerColliderHit c)
	{
		int goLayer = c.collider.gameObject.layer;
		if(goLayer == LayerMask.NameToLayer("Pickups"))
		{
			GameManager.Instance.PlayerPickupCollision(c);
		}
	}
	#endregion
	
	#region movement
	public void MoveOnGround(bool moveFwd, bool moveBack, bool moveRight, bool moveLeft)
	{
		Vector3 mvmntVec = Vector3.zero;
		
		if(moveFwd || moveBack)
		{
			Vector3 relFwd = gameObject.transform.TransformDirection(Vector3.forward);
			mvmntVec += relFwd * ((moveBack)?-1:1);
		}
		if(moveRight || moveLeft)
		{
			Vector3 relRight = gameObject.transform.TransformDirection(Vector3.right);
			mvmntVec += relRight * ((moveLeft)?-1:1);
		}
		
		mvmntVec*=_data.speed;
		
		//apply gravity
		mvmntVec.y -= _data.gravity*Time.deltaTime;
		
		_charCtrl.Move(mvmntVec*Time.deltaTime);	
	}
	
	public void MoveUp(float speed)
	{
		gameObject.transform.Translate(Vector3.up*speed*Time.deltaTime);
	}
	
	public void RotateY(float movement)
	{
		gameObject.transform.Rotate(Vector3.up * _data.turnSpeed*movement, Space.Self);
	}
	
	public void RotateX(float movement)
	{
		Transform camTrans = transform.GetComponentInChildren<Camera>().transform;
		camTrans.Rotate(-Vector3.right * _data.turnSpeed*movement, Space.Self);
		float xRot = camTrans.rotation.eulerAngles.x;
		if(xRot < 180.0f && xRot > _data.maxVerticalLookAngle)
		{
			xRot = _data.maxVerticalLookAngle;
			camTrans.localRotation = Quaternion.Euler(xRot, 0.0f, 0.0f);
		}
		if(xRot > 180.0f && xRot < 360 - _data.maxVerticalLookAngle)
		{
			xRot = 360 - _data.maxVerticalLookAngle;
			camTrans.localRotation = Quaternion.Euler(xRot, 0.0f, 0.0f);
		}
	}
	#endregion

	public void Init(PlayerData data, Vector3 position)
	{
		if(data == null)
			_data = new PlayerData();
		else
			_data = data;

		_scoreDisplay = ((GameObject)GameObject.Instantiate(data.scoreDisplayPrefab, Vector3.zero, Quaternion.identity)).GetComponent<ScoreCubesBehaviour>();
		CharacterController ctrl = gameObject.GetComponent<CharacterController>();
		if(ctrl == null)
		{
			ctrl = gameObject.AddComponent<CharacterController>();
		}
		ctrl.height = 2.0f;
		ctrl.radius = 0.75f;
		
		gameObject.transform.position = position;
	}
	
	public void Activate()
	{
		gameObject.SetActive(true);
		GameObject tmp = (GameObject)GameObject.Instantiate(_data.scoreDisplayPrefab, (new Vector3(1000,0,1000)), Quaternion.identity);
		_scoreDisplay = tmp.GetComponent<ScoreCubesBehaviour>();
	}
	
	public void AddScore(Color c)
	{
		Vector3 red = Vector3.right;
		Vector3 blue = Vector3.forward;
		Vector3 yellow = Vector3.right + Vector3.up;
		Vector3 cv = new Vector3(c.r, c.g, c.b);
		
		if(cv == red && _redScore++ < 6)
		{
			_scoreDisplay.AddOne(ScoreCubesBehaviour.ScoreCubeColor.Red);
		}
		else if(cv == yellow && _yellowScore++ < 6)
		{
			_scoreDisplay.AddOne(ScoreCubesBehaviour.ScoreCubeColor.Yellow);
		}
		else if(cv == blue && _blueScore++ < 6)
		{
			_scoreDisplay.AddOne(ScoreCubesBehaviour.ScoreCubeColor.Blue);
		}
		
		if(!_endShown && _redScore>=6 && _yellowScore>=6 && _blueScore>=6)
		{
			_scoreDisplay.TurnOnExitSign();
			GameManager.Instance.ShowEnd();
			_endShown = true;
		}
	}

	public bool IsOnElevator
	{
		get { return _onElevator; }
		set { _onElevator = value; }
	}
}