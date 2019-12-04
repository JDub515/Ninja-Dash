using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb;
    private Vector2 touchStart;
    private Vector2 wallAngle;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int health;
    private bool secondDash;
    private float invincibilityFrames;
    private float spriteFlashTimer;
    private int bulletSpeed;
    private int dashSpeed;
    private float heightAchieved;
    private int extraPoints;
    private float timePassed;
    public bool disabled;
    private BoxCollider2D enemyDetector;
    public bool inBossRoom;
    public Animator swordAnimator;
    public GameObject sword;
    public GameObject trail;

    public AudioSource jumpAudio;
    public AudioSource damagedAudio;
    public AudioSource deathAudio;
    public AudioSource itemAudio;
    public AudioSource fireballAudio;

    public Image[] hearts;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject menu;
    public GameObject pauseButton;
    public GameObject itemMenu;
    public TextMeshProUGUI menuTitle;
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public GameObject fireworksPrefab;

    public Image itemMenuImage;
    public TextMeshProUGUI itemMenuName;
    public TextMeshProUGUI itemMenuDescription;

    private bool swordItem;
    private bool bootsItem;
    private bool doubleShotItem;
    private bool shieldItem;
    public bool shockwaveItem;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        wallAngle = Vector2.up;
        health = 3;
        secondDash = true;
        invincibilityFrames = 0;
        spriteFlashTimer = 0;
        bulletSpeed = 5;
        dashSpeed = 15;
        heightAchieved = 0;
        extraPoints = 0;
        timePassed = 0;
        disabled = false;
        enemyDetector = Camera.main.GetComponentInChildren<BoxCollider2D>();
        inBossRoom = false;

        swordItem = false;
        bootsItem = false;
        doubleShotItem = false;
        shockwaveItem = false;

        if (LevelCreation.gameMode == 2) {
            swordItem = true;
            animator.SetBool("Sword Sheathed", true);
            LevelCreation.uniqueItems.RemoveAt(3);
            LevelCreation.uniqueItems.RemoveAt(2);
            LevelCreation.uniqueItems.RemoveAt(0);
        } else if (LevelCreation.gameMode == 3) {
            bootsItem = true;
            trail.GetComponent<TrailRenderer>().startColor = Color.magenta;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (disabled) {
            return;
        }
        if (!inBossRoom) {
            if (animator.GetBool("Moving") || Mathf.Abs(Camera.main.transform.position.y - transform.position.y) < .05f) {
                Camera.main.transform.position = new Vector3(0, transform.position.y, -10);
            } else if (Camera.main.transform.position.y - transform.position.y > 0) {
                Camera.main.transform.position -= new Vector3(0, .05f, 0);
            } else {
                Camera.main.transform.position += new Vector3(0, .05f, 0);
            }
        }
        if (Input.touchCount > 0) {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) {
                touchStart = t.position;
            } else if (t.phase == TouchPhase.Ended) {
                if ((t.position - touchStart).magnitude > 100) {
                    if ((rb.velocity == Vector2.zero && !Physics2D.Raycast(rb.position, t.position - touchStart, 1.2f, LayerMask.GetMask("Wall")) && Vector2.Angle(t.position - touchStart, wallAngle) < 180) || secondDash || (bootsItem && rb.velocity != Vector2.zero)) {
                        jumpAudio.Play();
                        if (rb.velocity == Vector2.zero) {
                            secondDash = true;
                        } else {
                            secondDash = false;
                        }
                        rb.velocity = (t.position - touchStart).normalized * dashSpeed;
                        rb.gravityScale = 2;
                        rb.rotation = Mathf.Rad2Deg * Mathf.Atan2((t.position - touchStart).y, (t.position - touchStart).x) - 90;
                        animator.SetBool("Moving", true);
                        if ((t.position - touchStart).x < 0) {
                            spriteRenderer.flipX = true;
                        } else {
                            spriteRenderer.flipX = false;
                        }
                    }
                } else if (LevelCreation.gameMode != 2) {
                    Collider2D[] results = new Collider2D[10];
                    ContactFilter2D contactFilter = new ContactFilter2D();
                    contactFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
                    int enemyCount = enemyDetector.OverlapCollider(contactFilter, results);
                    if (enemyCount > 0) {
                        Collider2D closest = results[0];
                        Collider2D secondClosest = results[1];
                        float distance = 1000;
                        foreach (Collider2D result in results) {
                            if (result != null && !Physics2D.Linecast(transform.position, result.transform.position, LayerMask.GetMask("Wall")) && (result.transform.position - transform.position).magnitude < distance) {
                                if (result != results[0]) {
                                    secondClosest = closest;
                                    closest = result;
                                }
                                distance = (result.transform.position - transform.position).magnitude;
                            }
                        }
                        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                        Vector2 direction = (Vector2)closest.transform.position - (Vector2)transform.position;
                        bullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * bulletSpeed;
                        bullet.GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                        if (Vector2.SignedAngle(direction, wallAngle) < 0) {
                            spriteRenderer.flipX = true;
                        } else {
                            spriteRenderer.flipX = false;
                        }
                        if (doubleShotItem && enemyCount >= 2) {
                            bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                            direction = (Vector2)secondClosest.transform.position - (Vector2)transform.position;
                            bullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * bulletSpeed;
                            bullet.GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                        }
                        fireballAudio.Play();
                    }
                }
            } 
        }
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
        if (Camera.main.transform.position.y > heightAchieved) {
            heightAchieved = Camera.main.transform.position.y;
        }
        timePassed += Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(extraPoints + heightAchieved - timePassed / 2).ToString();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("Wall") && animator.GetBool("Moving")) {
            if (Mathf.Abs(collision.contacts[0].normal.x) > Mathf.Abs(collision.contacts[0].normal.y)) {
                if (collision.relativeVelocity.x * collision.contacts[0].normal.x < 0) {
                    return;
                }
            } else {
                if (collision.relativeVelocity.y * collision.contacts[0].normal.y < 0) {
                    return;
                }
            }
            rb.velocity = Vector2.zero;
            wallAngle = collision.contacts[0].normal;
            rb.gravityScale = 0;
            rb.rotation = Mathf.Rad2Deg * Mathf.Atan2(wallAngle.y, wallAngle.x) - 90;
            animator.SetBool("Moving", false);
            if (!Physics2D.Raycast(rb.position, -collision.contacts[0].normal, .65f, LayerMask.GetMask("Wall"))) {
                rb.position = collision.contacts[0].point + collision.contacts[0].normal.normalized * .65f;
            }
            secondDash = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Sword Item")) {
            SavedData.challengeUnlocks[1]++;
            swordItem = true;
            animator.SetBool("Sword Sheathed", true);
            pickupItem(collision);
        } else if (collision.CompareTag("Boots Item")) {
            SavedData.challengeUnlocks[2]++;
            bootsItem = true;
            trail.GetComponent<TrailRenderer>().startColor = Color.magenta;
            pickupItem(collision);
        } else if (collision.CompareTag("Double Shot Item")) {
            doubleShotItem = true;
            pickupItem(collision);
        } else if (collision.CompareTag("Speed Shot Item")) {
            bulletSpeed += 3;
            pickupItem(collision);
        } else if (collision.CompareTag("Shield Item")) {
            shieldItem = true;
            pickupItem(collision);
        } else if (collision.CompareTag("Explosive Powder Item")) {
            shockwaveItem = true;
            pickupItem(collision);
        } else if (collision.CompareTag("Health Item")) {
            itemAudio.Play();
            if (health < 3) {
                hearts[health].enabled = true;
                health++;
            }
            ExtraPoints(5);
            Destroy(collision.gameObject);
        } else if (swordItem && collision.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<EnemyController>().Killed(true);
            KilledEnemy(5);
            StartCoroutine("SwordRotation", Quaternion.FromToRotation(Vector3.up, collision.transform.position - sword.transform.position));
            swordAnimator.SetTrigger("Start Swing");
            animator.SetBool("Sword Sheathed", false);
        } else if (swordItem && collision.CompareTag("Boss")) {
            collision.gameObject.GetComponent<BossController>().TakeDamage();
            StartCoroutine("SwordRotation", Quaternion.FromToRotation(Vector3.up, collision.transform.position - sword.transform.position));
            swordAnimator.SetTrigger("Start Swing");
            animator.SetBool("Sword Sheathed", false);
        } else if (collision.CompareTag("Spiked Wall")) {
            StartCoroutine("MenuPopup", 1);
            spriteRenderer.enabled = false;
            sword.SetActive(false);
            trail.SetActive(false);
            rb.bodyType = RigidbodyType2D.Static;
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            deathAudio.Play();
            disabled = true;
            FinalScore();
        }
    }

    private void pickupItem(Collider2D collision) {
        itemAudio.Play();
        pauseButton.SetActive(false);
        SetItemMenu(collision.GetComponent<Item>());
        itemMenu.SetActive(true);
        Time.timeScale = 0;
        disabled = true;
        Destroy(collision.gameObject);
    }

    public void RecieveDamage() {
        if (invincibilityFrames <= 0) {
            invincibilityFrames = shieldItem ? 3 : 1;
            health--;
            hearts[health].enabled = false;
            if (health > 0) {
                damagedAudio.Play();
            } else if (health == 0) {
                StartCoroutine("MenuPopup", 1);
                spriteRenderer.enabled = false;
                sword.SetActive(false);
                trail.SetActive(false);
                rb.bodyType = RigidbodyType2D.Static;
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                deathAudio.Play();
                disabled = true;
                FinalScore();
                SavedData.SaveChallengeUnlocks();
            }
        }
    }

    public void KilledEnemy(int x) {
        ExtraPoints(x);
        deathAudio.Play();
    }

    public void ExtraPoints(int x) {
        extraPoints += x;
    }

    private IEnumerator MenuPopup(float time) {
        pauseButton.SetActive(false);
        yield return new WaitForSeconds(time);
        menu.SetActive(true);
        disabled = true;
    }

    private void SetItemMenu (Item i) {
        itemMenuImage.sprite = i.image;
        itemMenuName.text = i.itemName;
        itemMenuDescription.text = i.description;
    }

    private IEnumerator SwordRotation(Quaternion q) {
        sword.transform.rotation = q;
        yield return null;
        while (swordAnimator.GetCurrentAnimatorStateInfo(0).IsName("Swing")) {
            sword.transform.rotation = q;
            yield return null;
        }
        animator.SetBool("Sword Sheathed", true);
    }

    public void Victory() {
        Collider2D[] results = Physics2D.OverlapBoxAll(Camera.main.transform.position, new Vector2 (11, 20), 0, LayerMask.GetMask("Shuriken"));
        foreach (Collider2D result in results) {
            result.GetComponent<BulletController>().DestroySelf();
        }
        if (LevelCreation.gameMode == 0) {
            SavedData.challengeUnlocks[3]++;
            SavedData.SaveChallengeUnlocks();
        }
        Instantiate(fireworksPrefab, new Vector3(0, Camera.main.transform.position.y - 11, 0), Quaternion.identity);
        menuTitle.text = "Victory!!!";
        StartCoroutine("MenuPopup", 4);
        FinalScore();
    }

    private void FinalScore() {
        finalScoreText.text = "Score: " + Mathf.FloorToInt(extraPoints + heightAchieved - timePassed / 2);
        if (Mathf.FloorToInt(extraPoints + heightAchieved - timePassed / 2) > SavedData.highScore[LevelCreation.gameMode]) {
            SavedData.highScore[LevelCreation.gameMode] = Mathf.FloorToInt(extraPoints + heightAchieved - timePassed / 2);
            SavedData.SaveHighScore();
            highScoreText.text = "NEW HIGH SCORE: " + SavedData.highScore[LevelCreation.gameMode] + "!!!";
        } else {
            highScoreText.text = "High Score: " + SavedData.highScore[LevelCreation.gameMode];
        }
    }
}
