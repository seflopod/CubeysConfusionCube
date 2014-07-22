using UnityEngine;
using System.Collections;

public class ElevatorBehaviour : MonoBehaviour
{
	private bool _moveUp = false;
	private static bool _atEnd = true;
	private bool _deactivate = true;
	private float _elevatorEndTimeout = 0.5f;
	private float _elevatorEndTimer = 0.0f;


	private void Start()
	{
		//I still think this should be set elsewhere, but for now we'll try this
		StartPosition = transform.position;
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
				Debug.Log("Timed out");
				GameManager.Instance.ElevatorTrigger(this, false);
				DeactivateElevator();
			}
		}
	}

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
		//If an elevator alread exists, we will not activate a new one
		/*if(!_deactivateCurrent)
		{
			return;
		}*/

		//setup color info
		MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
		mr.enabled = true;
		mr.materials[0].SetColor("_Color", GameManager.Instance.ElevatorColor);
		
		//prepare for movement
		Speed = GameManager.Instance.elevatorData.defaultSpeed;
		_moveUp = true;
		gameObject.GetComponent<Collider>().enabled = true;
		
		//administrative for (de)activating
		_atEnd = false;
		_deactivate = false;
	}

	public void DeactivateElevator()
	{
		if(!_deactivate)
		{
			Debug.Log("Deactiving previous elevator");
			gameObject.GetComponent<Collider>().enabled = false;
			gameObject.GetComponent<MeshRenderer>().enabled = false;
			gameObject.transform.position = StartPosition;
			_deactivate = true;
			_atEnd = false;
			_elevatorEndTimer = 0f;
		}
	}
	#endregion
	private void OnTriggerEnter(Collider c)
	{
		Debug.Log("Something triggered");
		if(c.CompareTag("Player"))
		{
			Debug.Log("Player triggered");
			GameManager.Instance.ElevatorTrigger(this, true);
			ActivateElevator();
		}
	}
	
	private void OnTriggerExit(Collider c)
	{
		Debug.Log("Something untriggered");
		if(c.CompareTag("Player"))
		{
			Debug.Log("Player untriggered");
			GameManager.Instance.ElevatorTrigger(this, false);
			DeactivateElevator();
		}
	}

	public static bool IsAtEnd { get { return _atEnd; } }
	public Vector3 StartPosition { get; private set; }
	public float Speed { get; set; }
}
