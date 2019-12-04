using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    protected Rigidbody2D rb;
    protected Vector2 wallAngle;
    public float waitTime;
    protected float shootDelay;
    protected GameObject player;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public GameObject healthItemPrefab;
    public GameObject shockwavePrefab;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    protected virtual void Start () {
        waitTime = 3;
        shootDelay = Random.Range(.5f, 1f);
        player = GameObject.FindGameObjectWithTag("Player");
        Vector2 angle = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.velocity = angle.normalized * 15;
        rb.rotation = Mathf.Rad2Deg * Mathf.Atan2(angle.y, angle.x) - 90;
        animator.SetBool("Moving", true);
        if (angle.x < 0) {
            spriteRenderer.flipX = true;
        } else {
            spriteRenderer.flipX = false;
        }
    }

    // Update is called once per frame
    void Update() {
        Jump();
        Shoot();
    }

    protected virtual void Jump() {
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
                waitTime = Random.Range(2f, 4f);
            }
        }
    }

    protected virtual void Shoot() {
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPoint.y > 0 && viewportPoint.y < 1) {
            shootDelay -= Time.deltaTime;
        }
        if (shootDelay <= 0) {
            if (rb.velocity == Vector2.zero) {
                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;
                bullet.GetComponent<Rigidbody2D>().velocity = LevelCreation.gameMode == 4 ? direction.normalized * 7 : direction.normalized * 5;
                bullet.GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                shootDelay = 2;
            }
        }
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
            rb.rotation = Mathf.Rad2Deg * Mathf.Atan2(wallAngle.y, wallAngle.x) - 90;
            if (!Physics2D.Raycast(rb.position, -collision.contacts[0].normal, .65f, LayerMask.GetMask("Wall"))) {
                rb.position = collision.contacts[0].point + collision.contacts[0].normal.normalized * .65f;
            }
            animator.SetBool("Moving", false);
            if (player != null) {
                if (Vector2.SignedAngle((Vector2)player.transform.position - (Vector2)transform.position, wallAngle) < 0) {
                    spriteRenderer.flipX = true;
                } else {
                    spriteRenderer.flipX = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Spiked Wall")) {
            player.GetComponent<PlayerController>().KilledEnemy(0);
            Killed(false);
        }
    }

    public void Killed(bool healthDrop) {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.transform.localScale = transform.localScale;
        if (healthDrop && Random.Range(0, 10) == 0) {
            Instantiate(healthItemPrefab, transform.position, Quaternion.identity);
        }
        if (player.GetComponent<PlayerController>().shockwaveItem) {
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
