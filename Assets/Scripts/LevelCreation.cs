using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreation : MonoBehaviour {

    public static int gameMode;

    public static List<Item> uniqueItems;
    public Item[] possibleUniqueItems;

    public GameObject wallPrefab;
    public GameObject[] horizontalWallPrefab;
    public GameObject[] horizontalEndCapLeftPrefab;
    public GameObject[] horizontalEndCapRightPrefab;
    public GameObject[] verticalWallPrefab;
    public GameObject[] verticalEndCapTopPrefab;
    public GameObject[] verticalEndCapBottomPrefab;
    public GameObject[] bossRoomPrefab;
    public GameObject[] enemyPrefabs;
    public GameObject player;
    public GameObject spikedWall;

    private int tileset;

    private int previousHeight;
    private int previousBlock;
    private int previousEnemy;
    private GameObject tempWall;
    private GameObject gameBoard;

    private int difficulty;
    public int bossDifficulty;

	// Use this for initialization
	void Start () {
        uniqueItems = new List<Item>();
        uniqueItems.AddRange(possibleUniqueItems);
        difficulty = 1;
        bossDifficulty = 0;
        gameBoard = new GameObject();
        previousBlock = 5;
        previousEnemy = 20;
        tileset = Random.Range(0, 1);
        CreateHorizontalWall(new Vector2(0, 0), new Vector2(12, 1));
        if (gameMode == 1) {
            CreateBossRoom(0, 24);
            bossDifficulty--;
        } else if (gameMode == 3) {
            CreateLevel(0, 100);
        } else {
            CreateLevel(0, 100);
            CreateBossRoom(100, 124);
        }
        if (gameMode == 3 || gameMode == 4) {
            Instantiate(spikedWall, new Vector3(0, -10, 0), Quaternion.identity);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (player.transform.position.y > previousHeight - 40) {
            tileset = Random.Range(0, 1);
            if (gameMode == 1) {
                CreateBossRoom(previousHeight, previousHeight + 24);
            } else if (gameMode == 3) {
                CreateLevel(previousHeight, previousHeight + 100);
            } else {
                CreateLevel(previousHeight, previousHeight + 100);
                CreateBossRoom(previousHeight, previousHeight + 24);
            }
        }
    }

    void CreateBossRoom(int startingHeight, int maxHeight) {
        previousHeight = maxHeight;
        previousBlock = 5 + maxHeight;
        previousEnemy = 20 + maxHeight;
        CreateVerticalWall(new Vector2(5.5f, (startingHeight + maxHeight + 1f) / 2), new Vector2(1, maxHeight - startingHeight));
        CreateVerticalWall(new Vector2(-5.5f, (startingHeight + maxHeight + 1f) / 2), new Vector2(1, maxHeight - startingHeight));
        Instantiate(bossRoomPrefab[tileset], new Vector2(0, startingHeight), Quaternion.identity, gameBoard.transform);

        bossDifficulty++; 
    }

    void CreateLevel(int startingHeight, int maxHeight) {
        previousHeight = maxHeight;
        CreateVerticalWall(new Vector2(5.5f, (startingHeight + maxHeight + 1f) / 2), new Vector2(1, maxHeight - startingHeight));
        CreateVerticalWall(new Vector2(-5.5f, (startingHeight + maxHeight + 1f) / 2), new Vector2(1, maxHeight - startingHeight));

        if (tempWall) {
            Collider2D[] results2 = Physics2D.OverlapBoxAll(tempWall.transform.position, new Vector2(11, 2), 0, LayerMask.GetMask("Enemy"));
            foreach (Collider2D result in results2) {
                result.GetComponent<EnemyController>().waitTime = 0;
            }
            Destroy(tempWall);
        }
        tempWall = Instantiate(wallPrefab, new Vector2(0, maxHeight), Quaternion.identity, gameBoard.transform);
        tempWall.GetComponent<SpriteRenderer>().size = new Vector2(12, 1);

        int i = previousBlock;
        while (i < maxHeight) {
            int wallHeight;
            int wallWidth;
            if (Random.Range(0, 2) == 0) {
                wallHeight = Random.Range(3, 8);
                wallWidth = Random.Range(1, 2);
                if (Random.Range(0, 2) == 0) {
                    CreateCappedVerticalWall(new Vector2(Random.Range(-4f, 4f), i + (wallHeight / 2)), new Vector2(wallWidth, wallHeight));
                } else {
                    CreateCappedVerticalWall(new Vector2(Random.Range(-4f, -1f), i + (wallHeight / 2)), new Vector2(wallWidth, wallHeight));
                    wallHeight = Random.Range(wallHeight, 8);
                    wallWidth = Random.Range(1, 2);
                    CreateCappedVerticalWall(new Vector2(Random.Range(1f, 4f), i + (wallHeight / 2)), new Vector2(wallWidth, wallHeight));
                }
            } else {
                wallHeight = Random.Range(1, 2);
                wallWidth = Random.Range(3, 8);
                CreateCappedHorizontalWall(new Vector2(Random.Range(-4f, 4f), i + (wallHeight / 2)), new Vector2(wallWidth, wallHeight));
            }
            i += wallHeight + Random.Range(2, 4);
        }
        previousBlock = i;
        Collider2D[] results = new Collider2D[1];
        i = previousEnemy;
        while (i < maxHeight) {
            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, difficulty)], new Vector3(Random.Range(-4, 5), i, 0), Quaternion.identity, gameBoard.transform);
            if (enemy.GetComponent<Collider2D>().OverlapCollider(new ContactFilter2D(), results) > 0) {
                Destroy(enemy);
                i += 1;
            } else {
                i += 5;
            }
        }
        previousEnemy = i;

        if (difficulty < 5) {
            difficulty++;
        }
    }

    void CreateWall(Vector2 center, Vector2 scale) {
        GameObject wall = Instantiate(wallPrefab, center, Quaternion.identity, gameBoard.transform);
        wall.GetComponent<SpriteRenderer>().size = scale;
    }

    void CreateVerticalWall(Vector2 center, Vector2 scale) {
        GameObject wall = Instantiate(verticalWallPrefab[tileset], center, Quaternion.identity, gameBoard.transform);
        wall.GetComponent<SpriteRenderer>().size = scale;
    }

    void CreateHorizontalWall(Vector2 center, Vector2 scale) {
        GameObject wall = Instantiate(horizontalWallPrefab[tileset], center, Quaternion.identity, gameBoard.transform);
        wall.GetComponent<SpriteRenderer>().size = scale;
    }

    void CreateCappedVerticalWall(Vector2 center, Vector2 scale) {
        GameObject wall = Instantiate(verticalWallPrefab[tileset], center, Quaternion.identity, gameBoard.transform);
        wall.GetComponent<SpriteRenderer>().size = new Vector2(scale.x, scale.y - 2);
        wall = Instantiate(verticalEndCapTopPrefab[tileset], new Vector2(center.x, center.y + (scale.y - 1f) / 2), Quaternion.identity, gameBoard.transform);
        wall.GetComponent<SpriteRenderer>().size = new Vector2(scale.x, 1);
        wall = Instantiate(verticalEndCapBottomPrefab[tileset], new Vector2(center.x, center.y - (scale.y - 1f) / 2), Quaternion.identity, gameBoard.transform);
        wall.GetComponent<SpriteRenderer>().size = new Vector2(scale.x, 1);
    }

    void CreateCappedHorizontalWall(Vector2 center, Vector2 scale) {
        GameObject wall = Instantiate(horizontalWallPrefab[tileset], center, Quaternion.identity, gameBoard.transform);
        wall.GetComponent<SpriteRenderer>().size = new Vector2(scale.x - 2, scale.y);
        wall = Instantiate(horizontalEndCapRightPrefab[tileset], new Vector2(center.x + (scale.x - 1f) / 2, center.y), Quaternion.identity, gameBoard.transform);
        wall.GetComponent<SpriteRenderer>().size = new Vector2(1, scale.y);
        wall = Instantiate(horizontalEndCapLeftPrefab[tileset], new Vector2(center.x - (scale.x - 1f) / 2, center.y), Quaternion.identity, gameBoard.transform);
        wall.GetComponent<SpriteRenderer>().size = new Vector2(1, scale.y);
    }
}
