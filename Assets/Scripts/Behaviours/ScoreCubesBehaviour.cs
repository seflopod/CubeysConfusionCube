using UnityEngine;
using System.Collections;

public class ScoreCubesBehaviour : MonoBehaviour
{
	public enum ScoreCubeColor
	{
		Red = 0x00,
		Yellow = 0x01,
		Blue = 0x02
	};
	
	private ScoreCubeBehaviour[] _cubes;
	
	private void Awake()
	{
		_cubes = new ScoreCubeBehaviour[3];
		_cubes[0] = transform.FindChild("RedScoreCube").gameObject.GetComponent<ScoreCubeBehaviour>();
		_cubes[1] = transform.FindChild("YellowScoreCube").gameObject.GetComponent<ScoreCubeBehaviour>();
		_cubes[2] = transform.FindChild("BlueScoreCube").gameObject.GetComponent<ScoreCubeBehaviour>();
	}
	
	public void AddOne(ScoreCubesBehaviour.ScoreCubeColor color)
	{
		_cubes[(int)color].AddOne();
	}
	
	public void SubtractOne(ScoreCubesBehaviour.ScoreCubeColor color)
	{
		_cubes[(int)color].SubtractOne();
	}
	
	public void TurnOnExitSign()
	{
		//not very scalable
		gameObject.GetComponentInChildren<GUITexture>().enabled = true;
	}
	
	public void TurnOffExitSign()
	{
		//not very scalable
		gameObject.GetComponentInChildren<GUITexture>().enabled = false;
	}
}
