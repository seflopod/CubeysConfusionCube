using UnityEngine;
using System.Collections;

public class GUITimerBehaviour : MonoBehaviour
{
	private int min = 0;
	private int sec = 0;
	private TextMesh _text;

	private void Start()
	{
		_text = GameObject.FindGameObjectWithTag("timer").GetComponent<TextMesh>();
	}

	private void Update()
	{
		float t = Time.timeSinceLevelLoad;

		min = Mathf.FloorToInt(t/60f);
		sec = Mathf.FloorToInt(t - 60f*min);

		_text.text = string.Format("{0,2:D2}:{1,2:D2}",min,sec);
	}
}
