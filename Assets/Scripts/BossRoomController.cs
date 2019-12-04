using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomController : MonoBehaviour {

    public GameObject bossPrefab;
    public GameObject poofPrefab;
    private GameObject boss;
    private bool bossCreated;
    public GameObject[] itemPrefabs;
    private bool itemsSpawned;
    public GameObject explosionPrefab;

    // Use this for initialization
    void Start () {
        bossCreated = false;
        itemsSpawned = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(bossCreated && boss == null && !itemsSpawned) {
            if (LevelCreation.uniqueItems.Count > 0) {
                int itemIndex = Random.Range(0, LevelCreation.uniqueItems.Count);
                Instantiate(LevelCreation.uniqueItems[itemIndex], new Vector3(0, transform.position.y + 15, 0), Quaternion.identity, transform);
                LevelCreation.uniqueItems.RemoveAt(itemIndex);
            } else {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Victory();
            }
            //Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], new Vector3(-2, transform.position.y + 15, 0), Quaternion.identity);
            //Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], new Vector3(2, transform.position.y + 15, 0), Quaternion.identity);
            itemsSpawned = true;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!bossCreated && collision.CompareTag("Player")) {
            Collider2D[] results = Physics2D.OverlapBoxAll(Camera.main.transform.position, new Vector2(11, 20), 0, LayerMask.GetMask("Shuriken"));
            foreach (Collider2D result in results) {
                result.GetComponent<BulletController>().DestroySelf();
            }
            results = Physics2D.OverlapBoxAll(Camera.main.transform.position, new Vector2(11, 20), 0, LayerMask.GetMask("Enemy"));
            foreach (Collider2D result in results) {
                result.GetComponent<EnemyController>().Killed(false);
            }
            if (results.Length > 0) {
                collision.GetComponent<PlayerController>().KilledEnemy(0);
            }
            GetComponent<Animator>().SetBool("Entered Boss Room", true);
            boss = Instantiate(bossPrefab, transform.position + new Vector3(0, 20, 0), Quaternion.identity);
            Instantiate(poofPrefab, transform.position + new Vector3(0, 20, 0), Quaternion.identity);
            collision.GetComponent<PlayerController>().inBossRoom = true;
            Camera.main.transform.position = new Vector3(0, transform.position.y + 15, -10);
            bossCreated = true;
        }
    }

    public void OpenExit() {
        Collider2D[] results = Physics2D.OverlapBoxAll(Camera.main.transform.position, new Vector2(11, 20), 0, LayerMask.GetMask("Enemy"));
        foreach (Collider2D result in results) {
            result.GetComponent<EnemyController>().waitTime = 0;
        }
        GetComponent<Animator>().SetBool("Defeated Boss", true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().inBossRoom = false;
        gameObject.GetComponent<BossRoomController>().enabled = false;
    }
}
