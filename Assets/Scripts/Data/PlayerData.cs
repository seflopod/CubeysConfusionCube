using UnityEngine;

[System.Serializable]
public class PlayerData
{
	public GameObject playerPrefab = null;
	public GameObject scoreDisplayPrefab = null;
	public float speed = 25.0f;
	public float gravity = 200.0f;
	public float turnSpeed = 0.1f; //deg per tick
	public float maxVerticalLookAngle = 60.0f;
}
