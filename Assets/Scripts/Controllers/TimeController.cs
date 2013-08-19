using UnityEngine;
using System.Collections;

public class TimeController : MonoBehaviour
{
	private bool _finishInit = false;
	
	private void Update()
	{
		if(!_finishInit && Time.timeSinceLevelLoad >= 2.0f)
			_finishInit = GameManager.Instance.FinishInit();
	}
	
	private void FixedUpdate()
	{
		GameManager.Instance.MoveElevator();
	}
}
