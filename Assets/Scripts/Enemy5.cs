using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy5 : EnemyController {

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
                GameObject bullet;
                Vector2 direction;
                foreach (float x in new float[] { 0, -.2f, .2f, -.4f, .4f }) {
                    bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    direction = (Vector2)player.transform.position - (Vector2)transform.position;
                    direction = new Vector2(Mathf.Cos(Mathf.Atan2(direction.y, direction.x) - x), Mathf.Sin(Mathf.Atan2(direction.y, direction.x) - x));
                    bullet.GetComponent<Rigidbody2D>().velocity = LevelCreation.gameMode == 4 ? direction.normalized * 7 : direction.normalized * 5;
                    bullet.GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                }
                shootDelay = 2;
            }
        }
    }
}
