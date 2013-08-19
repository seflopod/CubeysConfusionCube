using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
{
	private void FixedUpdate()
	{
		RaycastHit hit;
		int mask = (1 << 8) & (1 << 9);
		mask = ~mask; //everything but maze and self
		if(Physics.Raycast(transform.position, -Vector3.up, out hit, 8.0f, mask))
		{
			//only trigger in scene is the elevator trigger so assume
			if(hit.collider.isTrigger)
			{
				//print ("Player triggered!");
				//GameManager.Instance.ElevatorTrigger(hit.collider.transform.position);
			}
		}
			
	}
	
	private void OnControllerColliderHit(ControllerColliderHit c)
	{
		//ignore ground collisions
		//should actually ignore anything that's maze, but whatever
		//put this here to filter out calls to the manager
		if(!string.Equals("Bottom", c.collider.gameObject.name))
			GameManager.Instance.PlayerCollideHandle(c);
	}
}
