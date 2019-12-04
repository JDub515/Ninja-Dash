using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4 : EnemyController {
    private float previousAngle;
    private int previousAngleDirection;

    protected override void Start() {
        base.Start();
        previousAngle = -.4f;
        previousAngleDirection = 1;
    }

    void Update() {
        Jump();
        Shoot();
    }

    protected override void Shoot() {
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPoint.y > 0 && viewportPoint.y < 1) {
            shootDelay -= Time.deltaTime;
        }
        if (shootDelay <= 0) {
            if (rb.velocity == Vector2.zero) {
                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;
                direction = new Vector2(Mathf.Cos(Mathf.Atan2(direction.y, direction.x) - previousAngle), Mathf.Sin(Mathf.Atan2(direction.y, direction.x) - previousAngle));
                bullet.GetComponent<Rigidbody2D>().velocity = LevelCreation.gameMode == 4 ? direction.normalized * 7 : direction.normalized * 5;
                bullet.GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                previousAngle += .1f * previousAngleDirection;
                if (LevelCreation.gameMode == 2) {
                    previousAngle += .1f * previousAngleDirection;
                }
                if (previousAngle >= .4f || previousAngle <= -.4f) {
                    previousAngleDirection *= -1;
                    shootDelay = 1f;
                } else {
                    shootDelay = .1f;
                }
                if (LevelCreation.gameMode == 2) {
                    shootDelay *= 2;
                }
            }
        }
    }
}
