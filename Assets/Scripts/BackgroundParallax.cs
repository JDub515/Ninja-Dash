using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour {

    public GameObject[] backgrounds;
    public float speed;

    private int centerBackground;

	// Use this for initialization
	void Start () {
        centerBackground = 0;
	}

    void Update() {

    }

    void LateUpdate () {
        transform.position = (Vector2)Camera.main.transform.position * speed;
        if (Camera.main.transform.position.y - backgrounds[centerBackground].transform.position.y > 9.6f) {
            backgrounds[(centerBackground + 2) % 3].transform.position = backgrounds[(centerBackground + 2) % 3].transform.position + new Vector3(0, 57.6f, 0);
            centerBackground = (centerBackground + 1) % 3;
        } else if (Camera.main.transform.position.y - backgrounds[centerBackground].transform.position.y < -9.6f) {
            backgrounds[(centerBackground + 1) % 3].transform.position = backgrounds[(centerBackground + 1) % 3].transform.position - new Vector3(0, 57.6f, 0);
            centerBackground = (centerBackground + 2) % 3;
        }
    }
}
