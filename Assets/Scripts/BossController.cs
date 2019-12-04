using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController {
    private int shotType;
    private float shotTypeDelay;
    private int maxShotType;
    private int health;
    private float previousAngle;
    private int previousAngleDirection;
    private float invincibilityFrames;
    private float spriteFlashTimer;

    public GameObject[] bossBulletPrefabs;

    public AudioSource damagedAudio;

    protected override void Start() {
        base.Start();
        maxShotType = Mathf.Min(GameObject.FindGameObjectWithTag("Level Controller").GetComponent<LevelCreation>().bossDifficulty - 2, 4);
        shotType = maxShotType;
        health = maxShotType + 3;
        previousAngle = -.5f;
        previousAngleDirection = 1;
        invincibilityFrames = 0;
        spriteFlashTimer = 0;
    }

    void Update() {
        Jump();
        Shoot();

        if (invincibilityFrames > 0) {
            invincibilityFrames -= Time.deltaTime;
            spriteFlashTimer -= Time.deltaTime;
            if (spriteFlashTimer <= 0) {
                spriteFlashTimer = .1f;
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
        } else {
            spriteRenderer.enabled = true;
        }
    }

    public void TakeDamage() {
        if (invincibilityFrames <= 0) {
            invincibilityFrames = 2;
            health--;
            if (health > 0) {
                damagedAudio.Play();
            } else if (health == 0) {
                SavedData.challengeUnlocks[0]++;
                player.GetComponent<PlayerController>().KilledEnemy(50);
                Killed(false);
            }
        }
    }

    protected override void Jump() {
        if (rb.velocity == Vector2.zero) {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0) {
                Vector2 angle;
                do {
                    angle = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                } while (Vector2.Angle(angle, wallAngle) > 90);
                rb.velocity = LevelCreation.gameMode == 4 ? angle.normalized * 20 : angle.normalized * 15;
                rb.rotation = Mathf.Rad2Deg * Mathf.Atan2(angle.y, angle.x) - 90;
                animator.SetBool("Moving", true);
                if (angle.x < 0) {
                    spriteRenderer.flipX = true;
                } else {
                    spriteRenderer.flipX = false;
                }
                waitTime = Random.Range(3f, 5f);
                shotType = Random.Range(0, maxShotType + 1);
            }
        }
    }

    protected override void Shoot() {
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPoint.y > 0 && viewportPoint.y < 1) {
            shootDelay -= Time.deltaTime;
        }
        if (shootDelay <= 0) {
            if (rb.velocity == Vector2.zero) {
                if (shotType == 0) {
                    GameObject bullet = Instantiate(bossBulletPrefabs[0], transform.position, Quaternion.identity);
                    Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;
                    bullet.GetComponent<Rigidbody2D>().velocity = LevelCreation.gameMode == 4 ? direction.normalized * 7 : direction.normalized * 5;
                    bullet.GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                    shootDelay = .5f;
                } else if (shotType == 1) {
                    GameObject bullet;
                    Vector2 direction;
                    foreach (float x in new float[] { 0, -.2f, .2f }) {
                        bullet = Instantiate(bossBulletPrefabs[1], transform.position, Quaternion.identity);
                        direction = (Vector2)player.transform.position - (Vector2)transform.position;
                        direction = new Vector2(Mathf.Cos(Mathf.Atan2(direction.y, direction.x) - x), Mathf.Sin(Mathf.Atan2(direction.y, direction.x) - x));
                        bullet.GetComponent<Rigidbody2D>().velocity = LevelCreation.gameMode == 4 ? direction.normalized * 7 : direction.normalized * 5;
                        bullet.GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                    }
                    shootDelay = 1;
                } else if (shotType == 2) {
                    GameObject bullet = Instantiate(bossBulletPrefabs[2], transform.position, Quaternion.identity);
                    Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;
                    bullet.GetComponent<Rigidbody2D>().velocity = LevelCreation.gameMode == 4 ? direction.normalized * 7 : direction.normalized * 5;
                    bullet.GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                    shootDelay = .2f;
                } else if (shotType == 3) {
                    GameObject bullet = Instantiate(bossBulletPrefabs[3], transform.position, Quaternion.identity);
                    Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;
                    direction = new Vector2(Mathf.Cos(Mathf.Atan2(direction.y, direction.x) - previousAngle), Mathf.Sin(Mathf.Atan2(direction.y, direction.x) - previousAngle));
                    bullet.GetComponent<Rigidbody2D>().velocity = LevelCreation.gameMode == 4 ? direction.normalized * 7 : direction.normalized * 5;
                    bullet.GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                    previousAngle += .1f * previousAngleDirection;
                    if (previousAngle >= .5f || previousAngle <= -.5f) {
                        previousAngleDirection *= -1;
                        shootDelay = 1f;
                    } else {
                        shootDelay = .1f;
                    }
                } else if (shotType == 4) {
                    GameObject bullet;
                    Vector2 direction;
                    foreach (float x in new float[] { 0, -.2f, .2f, -.4f, .4f }) {
                        bullet = Instantiate(bossBulletPrefabs[4], transform.position, Quaternion.identity);
                        direction = (Vector2)player.transform.position - (Vector2)transform.position;
                        direction = new Vector2(Mathf.Cos(Mathf.Atan2(direction.y, direction.x) - x), Mathf.Sin(Mathf.Atan2(direction.y, direction.x) - x));
                        bullet.GetComponent<Rigidbody2D>().velocity = LevelCreation.gameMode == 4 ? direction.normalized * 7 : direction.normalized * 5;
                        bullet.GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                    }
                    shootDelay = 1;
                }
                if (LevelCreation.gameMode == 2) {
                    shootDelay *= 3;
                }
            }
        }
    }
}
