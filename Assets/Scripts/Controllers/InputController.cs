using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour
{

	void Update ()
	{
		GameManager.Instance.HandleKeyboard(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
		GameManager.Instance.HandleEscape(Input.GetKey(KeyCode.Escape));
		GameManager.Instance.HandleMouse(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
	}
}
