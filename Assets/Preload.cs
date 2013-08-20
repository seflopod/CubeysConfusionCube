using UnityEngine;
using System.Collections;

public class Preload : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_WEBPLAYER
		if(Application.CanStreamedLevelBeLoaded(1))
			Application.LoadLevel(1);
#elif UNITY_STANDALONE
		Application.LoadLevel(1);
#endif
	
	}
}
