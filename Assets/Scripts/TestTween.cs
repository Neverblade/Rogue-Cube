using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTween : MonoBehaviour {

	// Use this for initialization
	void Start () {
        iTween.FadeTo(gameObject, 0, 5);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
