using UnityEngine;

public class ElevatorManager
{
	private bool _moveUp;
	private bool _die;
	private GameObject _elevatorGameObject;
	private GameObject _elevatorPrefab;
	private float _speed; //units per sec
	private ElevatorData _data;
	
	public ElevatorManager(ElevatorData data)
	{
		_data = data;
		_elevatorPrefab = data.prefab;
		_moveUp = false;
		_die = true;
		_speed = 0.0f;
	}
	
	public void MoveUp()
	{
		_elevatorGameObject.transform.Translate(Vector3.up*_speed*Time.deltaTime);
		CheckForEnd();
	}
	
	public void SpawnNewElevator(Vector3 position)
	{
		SpawnNewElevator(position, _data.defaultSpeed);
	}
	
	public void SpawnNewElevator(Vector3 position, float speed)
	{
		//If an elevator already exists, we will not spawn a new one
		if(!_die)
			return;
		
		_elevatorGameObject = (GameObject)GameObject.Instantiate(_elevatorPrefab, position, Quaternion.identity);
		if(_elevatorGameObject == null)
			Debug.LogError("Couldn't spawn elevator");
		
		GameObject.FindGameObjectWithTag("Player").transform.parent = _elevatorGameObject.transform;
		_elevatorGameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", ElevatorColor);
		_speed = speed;
		_die = false;
		_moveUp = true;
	}
	
	private void CheckForEnd()
	{
		Ray ray = new Ray(_elevatorGameObject.transform.position, Vector3.up);
		int mask = 1 << 8; //we only want to hit the maze with the ray
		if(Physics.Raycast(ray, 8.0f, mask)) //magic
		{
			_moveUp = false;
			if(!_die)
			{
				_die = true;
				RemoveChild();
				GameObject.Destroy(_elevatorGameObject, 0.5f);
			}
		}
	}
	
	public void RemoveChild()
	{
		if(_elevatorGameObject.transform.childCount > 0)
			_elevatorGameObject.transform.GetChild(0).parent = null;
	}
	public bool IsMovingUp { get { return _moveUp; } }
	public Color ElevatorColor { get; set; }
}