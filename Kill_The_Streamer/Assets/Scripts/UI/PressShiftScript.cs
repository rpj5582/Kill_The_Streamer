﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressShiftScript : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			this.gameObject.SetActive(false);
		}
	}
}
