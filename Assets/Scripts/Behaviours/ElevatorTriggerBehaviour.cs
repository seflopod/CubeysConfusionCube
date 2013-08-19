using UnityEngine;

public class ElevatorTriggerBehaviour : MonoBehaviour
{
	private void OnTriggerEnter(Collider c)
	{
		if(c.CompareTag("Player"))
			GameManager.Instance.ElevatorTrigger(transform.position, true);
	}
	
	private void OnTriggerLeave(Collider c)
	{if(c.CompareTag("Player"))
			GameManager.Instance.ElevatorTrigger(transform.position, false);
	}
}
