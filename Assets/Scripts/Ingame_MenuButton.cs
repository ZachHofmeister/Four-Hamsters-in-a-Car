using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingame_MenuButton : MonoBehaviour {


	public void changeScene (string scenename) {
		Application.LoadLevel (scenename);
	}
}
