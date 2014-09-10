using UnityEngine;
using System.Collections;

public class ElevatorBehaviour : MonoBehaviour
{
	private bool _moveUp = false;
	private static bool _atEnd = true;
	private bool _deactivate = true;
	private float _elevatorEndTimeout = 0.5f;
	private float _elevatorEndTimer = 0.0f;
	private PlayerBehaviour _player = null;

	#region monobehaviour
	private void Awake()
	{
		//I still think this should be set elsewhere, but for now we'll try this
		StartPosition = transform.position;
	}

	private void Start()
	{
		Speed = GameManager.Instance.elevatorData.defaultSpeed;
	}

	private void Update()
	{
		if(_moveUp)
		{
			MoveUp();
		}
		else if(_atEnd && !_deactivate)
		{
			_elevatorEndTimer+=Time.deltaTime;
			if(_elevatorEndTimer >= _elevatorEndTimeout)
			{
				//Debug.Log("Timed out");
				DeactivateElevator();
			}
		}
	}

	private void LateUpdate()
	{
		if(!renderer.enabled && transform.position != StartPosition)
		{
			transform.position = StartPosition;
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		_player = c.gameObject.GetComponent<PlayerBehaviour>();
		if(_player != null)
		{
			//Debug.Log("Player triggered");
			ActivateElevator();
		}
	}
	
	private void OnTriggerExit(Collider c)
	{
		_player = c.gameObject.GetComponent<PlayerBehaviour>();
		if(_player != null)
		{
			//Debug.Log("Player untriggered");
			DeactivateElevator();
		}
	}
	#endregion

	#region movement
	public void MoveUp()
	{
		transform.Translate(Vector3.up*Speed*Time.deltaTime);
		checkForEnd();
	}
	
	private void checkForEnd()
	{
		Ray ray = new Ray(transform.position, Vector3.up);
		int mask = 1 << LayerMask.NameToLayer("Maze");
		if(Physics.Raycast(ray, 8.0f, mask)) //magic, using 8m b/c each cell is 8x8x8
		{
			_moveUp = false;
			_atEnd = true;
		}
	}
	#endregion

	#region activation
	public void ActivateElevator()
	{
		//Don't try activating more than once
		if(!_deactivate)
		{
			return;
		}

		//setup color info
		MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
		renderer.enabled = true;
		renderer.materials[0].SetColor("_Color", GameManager.Instance.ElevatorColor);
		
		//prepare for movement
		Speed = GameManager.Instance.elevatorData.defaultSpeed;
		_moveUp = true;
		gameObject.GetComponent<Collider>().enabled = true;
		if(_player != null)
		{
			_player.IsOnElevator = true;
		}
		
		//administrative for (de)activating
		_atEnd = false;
		_deactivate = false;
	}

	public void DeactivateElevator()
	{
		//Debug.Log("Deactiving elevator");
		_deactivate = true;
		transform.position = StartPosition;
		gameObject.GetComponent<Collider>().enabled = false;
		renderer.enabled = false;
		_atEnd = false;
		_elevatorEndTimer = 0f;
		if(_player != null)
		{
			_player.IsOnElevator = false;
			_player = null;
		}
	}
	#endregion


	public static bool IsAtEnd { get { return _atEnd; } }
	public Vector3 StartPosition { get; private set; }
	public float Speed { get; set; }
}
