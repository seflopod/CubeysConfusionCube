using UnityEngine;

public class ElevatorManager
{
	private GameObject _currentElevatorGO;
	private ElevatorData _data;
	private bool _moveUp = false;
	private float _speed = 0f; //units per sec
	private bool _atEnd = false;
	private bool _deactivateCurrent = true;
	
	public ElevatorManager(ElevatorData data)
	{
		_data = data;
		_speed = _data.defaultSpeed;
	}
	
	public void MoveUp()
	{
		if(_currentElevatorGO != null)
		{
			_currentElevatorGO.transform.Translate(Vector3.up*_speed*Time.deltaTime);
			checkForEnd();
		}
	}

	private void checkForEnd()
	{
		Ray ray = new Ray(_currentElevatorGO.transform.position, Vector3.up);
		int mask = 1 << LayerMask.NameToLayer("Maze");
		if(Physics.Raycast(ray, 8.0f, mask)) //magic, using 8m b/c each cell is 8x8x8
		{
			_moveUp = false;
			_atEnd = true;
		}
	}

	public void ActivateElevator(GameObject elevatorGO)
	{
		//If an elevator alread exists, we will not activate a new one
		/*if(!_deactivateCurrent)
		{
			return;
		}*/

		if(elevatorGO == null)
		{
			Debug.LogError("Null elevator object");
			return;
		}

		_currentElevatorGO = elevatorGO;

		//setup color info
		MeshRenderer mr = _currentElevatorGO.GetComponent<MeshRenderer>();
		mr.enabled = true;
		mr.materials[0].SetColor("_Color", ElevatorColor);

		//prepare for movement
		_speed = _data.defaultSpeed;
		_moveUp = true;
		_currentElevatorGO.GetComponent<Collider>().enabled = true;

		//administrative for (de)activating
		_atEnd = false;
		_deactivateCurrent = false;
	}

	public void DeactivateElevator()
	{
		if(!_deactivateCurrent)
		{
			Debug.Log("Deactiving current elevator");
			_currentElevatorGO.GetComponent<Collider>().enabled = false;
			_currentElevatorGO.GetComponent<MeshRenderer>().enabled = false;
			_currentElevatorGO.transform.position = _currentElevatorGO.GetComponent<ElevatorBehaviour>().StartPosition;
			_deactivateCurrent = true;
			_atEnd = false;
		}
	}
	
	public bool IsMovingUp { get { return _moveUp; } }
	
	public Color ElevatorColor { get; set; }
	
	public float Speed { get { return _speed; } }

	public bool IsAtEnd { get { return _atEnd; } }

	public bool IsDeactivating { get { return _deactivateCurrent; } }
}