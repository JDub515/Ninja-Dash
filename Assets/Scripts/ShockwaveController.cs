using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveController : MonoBehaviour {

    public float speed;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        transform.localScale += new Vector3(speed, speed, speed);
        if (transform.localScale.x > .7f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Shuriken") && !collision.GetComponent<BulletController>().playerBullet) {
            collision.GetComponent<BulletController>().DestroySelf();
        }
    }
}
