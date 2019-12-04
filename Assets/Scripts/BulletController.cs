using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public bool playerBullet;
    public GameObject explosionPrefab;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().angularVelocity = 600;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Wall")) {
            Destroy(gameObject);
        } else if (playerBullet && collision.CompareTag("Enemy")) {
            Destroy(gameObject);
            collision.gameObject.GetComponent<EnemyController>().Killed(true); ;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().KilledEnemy(5);
        } else if (playerBullet && collision.CompareTag("Boss")) {
            Destroy(gameObject);
            collision.gameObject.GetComponent<BossController>().TakeDamage();
        } else if (!playerBullet && collision.CompareTag("Player")) {
            Destroy(gameObject);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().RecieveDamage();
        }
    }

    public void DestroySelf() {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(.4f, .4f, .4f);
        Destroy(gameObject);
    }
}
