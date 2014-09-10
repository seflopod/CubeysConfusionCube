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
		Camera cam = (GameObject.FindGameObjectWithTag("ScoreCam")).GetComponent<Camera>();
		float zDist = (transform.position - cam.gameObject.transform.position).z;
		Vector3 left = cam.ViewportToWorldPoint(new Vector3(0.0f, 0.5f, zDist));
		Vector3 right = cam.ViewportToWorldPoint(new Vector3(1.0f, 0.5f, zDist));
		float viewWidth = (right - left).magnitude;
		float scale = viewWidth/(3 * Mathf.Sqrt(2.0f));
		transform.localScale = new Vector3(scale, scale, scale);
	}
	
	public int AddOne(ScoreCubesBehaviour.ScoreCubeColor color)
	{
		return _cubes[(int)color].AddOne();
	}
	
	public int SubtractOne(ScoreCubesBehaviour.ScoreCubeColor color)
	{
		return _cubes[(int)color].SubtractOne();
	}
	
	public void TurnOnExitSign()
	{
		Color green = new Color(0.0f, 0.75f, 0.0f);
		for(int i=0;i<3;++i)
		{
			_cubes[i].ChangeColor(green);
		}
	}
}
