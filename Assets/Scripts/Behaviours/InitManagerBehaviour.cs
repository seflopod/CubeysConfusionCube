using UnityEngine;
using System.Collections;

public class InitManagerBehaviour : MonoBehaviour
{
	public PlayerData playerData;
	public MazeData mazeData;
	public MazeCellData mazeCellData;
	public ElevatorData elevatorData;
	
	private void Awake()
	{
		GameManager.Instance.Init(playerData, mazeData, mazeCellData, elevatorData);
	}
}
