using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MazeData
{
	public GameObject mazePrefab;
	public GameObject elevatorTriggerPrefab;
	public GameObject endPickupPrefab;
	public AudioClip endCellSound;
	public List<GameObject> pickups;
	public int numberOfRows = 8;
	public int numberOfColumns = 8;
	public int numberOfLayers = 8;
}