using UnityEngine;
using System.Collections;

public class ElevatorBehaviour : MonoBehaviour
{
	private void Start()
	{
		//I still think this should be set elsewhere, but for now we'll try this
		StartPosition = transform.position;
	}

	private void OnTriggerEnter(Collider c)
	{
		Debug.Log("Something triggered");
		if(c.CompareTag("Player"))
		{
			Debug.Log("Player triggered");
			GameManager.Instance.ElevatorTrigger(gameObject, true);
		}
	}
	
	private void OnTriggerExit(Collider c)
	{
		Debug.Log("Something untriggered");
		if(c.CompareTag("Player"))
		{
			Debug.Log("Player untriggered");
			GameManager.Instance.ElevatorTrigger(gameObject, false);
		}
	}

	public Vector3 StartPosition { get; private set; }
}
