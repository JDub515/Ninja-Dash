using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedWallController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (LevelCreation.gameMode == 4) {
            if (transform.position.y < Camera.main.transform.position.y - 11) {
                transform.position = new Vector3(0, Camera.main.transform.position.y - 11, 0);
            }
        } else {
            transform.position = transform.position + new Vector3(0, .1f, 0);
        }
	}
}
