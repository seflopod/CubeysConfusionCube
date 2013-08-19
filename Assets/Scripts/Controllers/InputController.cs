using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour
{

	void Update ()
	{
		GameManager.Instance.HandleKeyboard(Input.GetKey(KeyCode.W),
											Input.GetKey(KeyCode.S),
											Input.GetKey(KeyCode.D),
											Input.GetKey(KeyCode.A));
		GameManager.Instance.HandleEscape(Input.GetKey(KeyCode.Escape));
		GameManager.Instance.HandleMouse(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
	}
}
